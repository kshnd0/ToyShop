using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.Customer
{
    public partial class AddressDialog : Window
    {
        public string City { get; private set; }
        public string Street { get; private set; }
        public string House { get; private set; }
        public string Apartment { get; private set; }

        // Конструктор для добавления нового адреса
        public AddressDialog()
        {
            InitializeComponent();
        }

        // Конструктор для редактирования адреса
        public AddressDialog(Address address) : this()
        {
            tbCity.Text = address.City;
            tbStreet.Text = address.Street;
            tbHouse.Text = address.House;
            tbApartment.Text = address.Apartment;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
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

            City = tbCity.Text.Trim();
            Street = tbStreet.Text.Trim();
            House = tbHouse.Text.Trim();
            Apartment = tbApartment.Text.Trim();

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