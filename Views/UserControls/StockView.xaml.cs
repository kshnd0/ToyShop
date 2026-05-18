using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class StockView : UserControl
    {
        private ToyShopContext _context;
        private List<StockDisplay> _allStock;
        private Border _selectedCard;
        private int _selectedStockId = -1;

        public class StockDisplay
        {
            public int IdWarehouseProduct { get; set; }
            public string WarehouseName { get; set; } = "";
            public string ProductName { get; set; } = "";
            public int Amount { get; set; }
        }

        public StockView()
        {
            InitializeComponent();
            _context = new ToyShopContext();
            LoadData();
        }

        public void SetButtonsVisibility(Visibility visibility)
        {
            btnAdd.Visibility = visibility;
            btnEdit.Visibility = visibility;
            btnDelete.Visibility = visibility;
        }

        private void LoadData()
        {
            var stock = _context.WarehouseProducts.ToList();
            var warehouses = _context.Warehouses.ToDictionary(w => w.IdWarehouse, w => w.Name);
            var products = _context.Products.ToDictionary(p => p.IdProduct, p => p.Name);

            _allStock = stock.Select(s => new StockDisplay
            {
                IdWarehouseProduct = s.IdWarehouseProduct,
                WarehouseName = warehouses.ContainsKey(s.IdWarehouse) ? warehouses[s.IdWarehouse] : "Не указан",
                ProductName = products.ContainsKey(s.IdProduct) ? products[s.IdProduct] : "Не указан",
                Amount = s.Amount
            }).ToList();
            icStock.ItemsSource = _allStock;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icStock.ItemsSource = _allStock;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allStock
                    .Where(s => s.WarehouseName.ToLower().Contains(search) ||
                                s.ProductName.ToLower().Contains(search))
                    .ToList();
                icStock.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            var stockId = (int)card.Tag;

            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedStockId = stockId;
        }

        private StockDisplay GetSelectedStock()
        {
            if (_selectedStockId == -1) return null;
            return _allStock.FirstOrDefault(s => s.IdWarehouseProduct == _selectedStockId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var warehouses = _context.Warehouses.ToList();
            var products = _context.Products.ToList();

            var dialog = new StockDialog(warehouses, products);
            if (dialog.ShowDialog() == true)
            {
                var stock = new WarehouseProduct
                {
                    IdWarehouse = dialog.SelectedWarehouseId,
                    IdProduct = dialog.SelectedProductId,
                    Amount = dialog.Amount
                };
                _context.WarehouseProducts.Add(stock);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedStock();
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var stock = _context.WarehouseProducts.FirstOrDefault(s => s.IdWarehouseProduct == selected.IdWarehouseProduct);
            if (stock == null) return;

            var warehouses = _context.Warehouses.ToList();
            var products = _context.Products.ToList();

            var dialog = new StockDialog(warehouses, products, stock);
            if (dialog.ShowDialog() == true)
            {
                stock.IdWarehouse = dialog.SelectedWarehouseId;
                stock.IdProduct = dialog.SelectedProductId;
                stock.Amount = dialog.Amount;

                _context.WarehouseProducts.Update(stock);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Запись обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedStock();
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var stock = _context.WarehouseProducts.FirstOrDefault(s => s.IdWarehouseProduct == selected.IdWarehouseProduct);
            if (stock == null) return;

            if (MessageBox.Show($"Удалить запись: {selected.ProductName} на складе {selected.WarehouseName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.WarehouseProducts.Remove(stock);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Запись удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}