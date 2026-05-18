using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class StockDialog : Window
    {
        public int SelectedWarehouseId { get; private set; }
        public int SelectedProductId { get; private set; }
        public int Amount { get; private set; }

        public StockDialog(List<Warehouse> warehouses, List<Product> products, WarehouseProduct stock = null)
        {
            InitializeComponent();

            cbWarehouse.ItemsSource = warehouses;
            cbProduct.ItemsSource = products;

            if (stock != null)
            {
                cbWarehouse.SelectedValue = stock.IdWarehouse;
                cbProduct.SelectedValue = stock.IdProduct;
                tbAmount.Text = stock.Amount.ToString();
            }
            else
            {
                if (warehouses.Any()) cbWarehouse.SelectedIndex = 0;
                if (products.Any()) cbProduct.SelectedIndex = 0;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Выберите склад!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(tbAmount.Text, out int amount) || amount < 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedWarehouseId = (int)cbWarehouse.SelectedValue;
            SelectedProductId = (int)cbProduct.SelectedValue;
            Amount = amount;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}