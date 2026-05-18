using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class ProductDialog : Window
    {
        public string ProductName { get; private set; }
        public decimal ProductCost { get; private set; }
        public string Manufacturer { get; private set; }
        public int SelectedCategoryId { get; private set; }
        public int SelectedProviderId { get; private set; }

        public ProductDialog(List<Category> categories, List<Provider> providers, Product product = null)
        {
            InitializeComponent();

            cbCategory.ItemsSource = categories;
            cbProvider.ItemsSource = providers;

            if (product != null)
            {
                tbName.Text = product.Name;
                tbCost.Text = product.Cost.ToString();
                tbManufacturer.Text = product.Manufacturer;
                cbCategory.SelectedValue = product.IdCategory;
                cbProvider.SelectedValue = product.IdProvider;
            }

            if (categories.Any()) cbCategory.SelectedIndex = 0;
            if (providers.Any()) cbProvider.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(tbCost.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductName = tbName.Text.Trim();
            ProductCost = cost;
            Manufacturer = tbManufacturer.Text.Trim();
            SelectedCategoryId = (int)cbCategory.SelectedValue;
            SelectedProviderId = (int)cbProvider.SelectedValue;

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