using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ToyShop.Models;

namespace ToyShop.Views.Customer
{
    public partial class CartWindow : Window
    {
        private int _userId;
        private int _basketId;
        private ToyShopContext _context;
        private List<CartItemDisplay> _cartItems;

        public CartWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _context = new ToyShopContext();
            LoadCart();
        }

        private void LoadCart()
        {
            var basket = _context.Baskets.FirstOrDefault(b => b.IdUser == _userId);
            if (basket == null)
            {
                basket = new Basket { IdUser = _userId };
                _context.Baskets.Add(basket);
                _context.SaveChanges();
            }
            _basketId = basket.IdBasket;

            _cartItems = _context.BasketProducts
                .Include(bp => bp.IdProductNavigation)
                .Where(bp => bp.IdBasket == _basketId)
                .Select(bp => new CartItemDisplay
                {
                    IdBasketProduct = bp.IdBasketProduct,
                    IdProduct = bp.IdProduct,
                    ProductName = bp.IdProductNavigation.Name,
                    Price = bp.IdProductNavigation.Cost,
                    Quantity = bp.Amount,
                    ImagePath = $"/Images/Products/{bp.IdProduct}.jpg"
                }).ToList();

            icCartItems.ItemsSource = _cartItems;
            UpdateTotal();
            UpdateItemsCount();
        }

        private void UpdateTotal()
        {
            decimal total = _cartItems?.Sum(i => i.Total) ?? 0;
            tbTotal.Text = $"{total:F2} руб.";
        }

        private void UpdateItemsCount()
        {
            int count = _cartItems?.Count ?? 0;
            tbItemsCount.Text = $"{count} товаров";
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int basketProductId = (int)button.Tag;

            var item = _cartItems.FirstOrDefault(i => i.IdBasketProduct == basketProductId);
            if (item == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить \"{item.ProductName}\" из корзины?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var basketProduct = await _context.BasketProducts.FindAsync(basketProductId);
                if (basketProduct != null)
                {
                    _context.BasketProducts.Remove(basketProduct);
                    await _context.SaveChangesAsync();
                    LoadCart();
                }
            }
        }

        private async void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int basketProductId = (int)button.Tag;

            var basketProduct = await _context.BasketProducts.FindAsync(basketProductId);
            if (basketProduct == null) return;

            if (basketProduct.Amount > 1)
            {
                basketProduct.Amount--;
                await _context.SaveChangesAsync();

                var item = _cartItems.FirstOrDefault(i => i.IdBasketProduct == basketProductId);
                if (item != null)
                {
                    item.Quantity = basketProduct.Amount;
                    UpdateTotal();
                    // Обновляем отображение
                    icCartItems.ItemsSource = null;
                    icCartItems.ItemsSource = _cartItems;
                }
            }
            else
            {
                MessageBox.Show("Минимальное количество - 1. Для удаления используйте кнопку Удалить",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int basketProductId = (int)button.Tag;

            var basketProduct = await _context.BasketProducts.FindAsync(basketProductId);
            if (basketProduct != null)
            {
                basketProduct.Amount++;
                await _context.SaveChangesAsync();

                var item = _cartItems.FirstOrDefault(i => i.IdBasketProduct == basketProductId);
                if (item != null)
                {
                    item.Quantity = basketProduct.Amount;
                    UpdateTotal();
                    // Обновляем отображение
                    icCartItems.ItemsSource = null;
                    icCartItems.ItemsSource = _cartItems;
                }
            }
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems == null || _cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Оформление заказа",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkout = new CheckoutWindow(_userId, _cartItems);
            checkout.Owner = this;
            if (checkout.ShowDialog() == true)
            {
                LoadCart();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}