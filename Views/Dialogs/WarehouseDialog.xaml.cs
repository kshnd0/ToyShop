using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class WarehouseDialog : Window
    {
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string House { get; private set; }

        public WarehouseDialog(Warehouse warehouse = null)
        {
            InitializeComponent();

            if (warehouse != null)
            {
                tbName.Text = warehouse.Name;
                tbCity.Text = warehouse.City;
                tbStreet.Text = warehouse.Street;
                tbHouse.Text = warehouse.House;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название склада!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbCity.Text))
            {
                MessageBox.Show("Введите город!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbStreet.Text))
            {
                MessageBox.Show("Введите улицу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbHouse.Text))
            {
                MessageBox.Show("Введите номер дома!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Name = tbName.Text.Trim();
            City = tbCity.Text.Trim();
            Street = tbStreet.Text.Trim();
            House = tbHouse.Text.Trim();

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