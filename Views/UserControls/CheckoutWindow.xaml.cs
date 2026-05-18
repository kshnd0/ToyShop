using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToyShop.Models;
using ToyShop.Views.Main;

namespace ToyShop.Views.Customer
{
    public partial class CheckoutWindow : Window
    {
        private int _userId;
        private List<CartItemDisplay> _cartItems;
        private ToyShopContext _context;
        private readonly int _mainWarehouseId = 1;

        public class AddressDisplay
        {
            public int IdAddress { get; set; }
            public string DisplayName { get; set; }
        }

        public CheckoutWindow(int userId, List<CartItemDisplay> cartItems)
        {
            InitializeComponent();
            _userId = userId;
            _cartItems = cartItems;
            _context = new ToyShopContext();

            LoadAddresses();
            LoadItems();
            CalculateTotal();
        }

        private void LoadAddresses()
        {
            var addresses = _context.Addresses
                .Where(a => a.IdUser == _userId)
                .Select(a => new AddressDisplay
                {
                    IdAddress = a.IdAddress,
                    DisplayName = $"{a.City}, ул.{a.Street}, {a.House}" +
                                  (string.IsNullOrEmpty(a.Apartment) ? "" : $", кв.{a.Apartment}")
                }).ToList();

            cbAddress.ItemsSource = addresses;
            if (addresses.Any()) cbAddress.SelectedIndex = 0;
        }

        private void LoadItems()
        {
            var items = _cartItems.Select(i => new
            {
                i.ProductName,
                i.Quantity,
                Цена = $"{i.Price:F2} руб.",
                Сумма = $"{i.Total:F2} руб."
            }).ToList();

            dgItems.ItemsSource = items;
        }

        private void CalculateTotal()
        {
            decimal total = _cartItems?.Sum(i => i.Total) ?? 0;
            tbTotal.Text = $"{total:F2} руб.";
        }

        private void BtnAddAddress_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressDialog();
            if (dialog.ShowDialog() == true)
            {
                var address = new Address
                {
                    IdUser = _userId,
                    City = dialog.City,
                    Street = dialog.Street,
                    House = dialog.House,
                    Apartment = dialog.Apartment
                };

                _context.Addresses.Add(address);
                _context.SaveChanges();
                LoadAddresses();
            }
        }

        private int GetTotalProductStock(int productId) //проверка наличия товара(подсчитывает общее кол-во на всех складах)
        {
            return _context.WarehouseProducts
                .Where(wp => wp.IdProduct == productId)
                .Sum(wp => wp.Amount);
        }

        private bool DeductProductStock(int productId, int requestedQuantity, int mainWarehouseId)
        {
            //получаем остатки товара
            var stockItems = _context.WarehouseProducts
                .Where(wp => wp.IdProduct == productId && wp.Amount > 0)
                .OrderBy(wp => wp.IdWarehouse == mainWarehouseId ? 0 : 1)
                .ThenBy(wp => wp.IdWarehouse)
                .ToList();

            int remainingToDeduct = requestedQuantity;
            //проходим по всем складам
            foreach (var stock in stockItems)
            {
                if (remainingToDeduct <= 0) break; //все списали, выходим
                //берется минимум из (сколько есть на складе, сколько нужно)
                int deductAmount = Math.Min(stock.Amount, remainingToDeduct);
                stock.Amount -= deductAmount; //уменьшаем остаток на складе
                remainingToDeduct -= deductAmount; //уменьшаем сколько еще нужно 
            }

            //если не хватило товара
            if (remainingToDeduct > 0)
            {
                return false; //не удалось списать 
            }

            _context.SaveChanges(); //сохраняем изменения в бд
            return true; //все списали успешно
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbAddress.SelectedItem == null)
                {
                    MessageBox.Show("Выберите адрес доставки!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var item in _cartItems)
                {
                    int availableStock = GetTotalProductStock(item.IdProduct);
                    if (availableStock < item.Quantity)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.IdProduct == item.IdProduct);
                        MessageBox.Show($"Товара \"{product?.Name}\" недостаточно на складе!\n" +
                            $"Доступно: {availableStock}, Заказано: {item.Quantity}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var selectedPayMethod = cbPayMethod.SelectedItem as ComboBoxItem;
                string payMethod = selectedPayMethod?.Content.ToString() ?? "Наличные";

                var firstEmployee = _context.Employees.FirstOrDefault();
                int employeeId = firstEmployee?.IdEmployee ?? 1;

                var order = new Order
                {
                    RegistrationDate = DateTime.Now,
                    Status = "оформлен",
                    PayMethod = payMethod,
                    IdUser = _userId,
                    IdEmployee = employeeId
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in _cartItems)
                {
                    var orderItem = new ProductOrder
                    {
                        IdOrder = order.IdOrder,
                        IdProduct = item.IdProduct,
                        Amount = item.Quantity
                    };
                    _context.ProductOrders.Add(orderItem);

                    bool success = DeductProductStock(item.IdProduct, item.Quantity, _mainWarehouseId);
                    if (!success)
                    {
                        throw new Exception($"Не удалось списать товар {item.ProductName}");
                    }
                }

                //очищение корзины
                var basket = _context.Baskets.FirstOrDefault(b => b.IdUser == _userId);
                if (basket != null)
                {
                    var basketItems = _context.BasketProducts.Where(bp => bp.IdBasket == basket.IdBasket);
                    _context.BasketProducts.RemoveRange(basketItems);
                }

                await _context.SaveChangesAsync();

                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    mainWindow.RefreshOrders();
                }

                MessageBox.Show($"Заказ №{order.IdOrder} успешно оформлен!\nСпособ оплаты: {payMethod}\nСумма: {tbTotal.Text}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}