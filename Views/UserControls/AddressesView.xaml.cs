using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;
using ToyShop.Views.Customer;

namespace ToyShop.Views.UserControls
{
    public partial class AddressesView : UserControl
    {
        private ToyShopContext _context;
        private int _userId;
        private List<AddressDisplay> _allAddresses;
        private Border _selectedCard;
        private int _selectedAddressId = -1;

        public class AddressDisplay
        {
            public int IdAddress { get; set; }
            public string City { get; set; } = "";
            public string Street { get; set; } = "";
            public string House { get; set; } = "";
            public string Apartment { get; set; } = "";
            public string FullAddress => $"{City}, ул.{Street}, {House}" + (string.IsNullOrEmpty(Apartment) ? "" : $", кв.{Apartment}");
        }

        public AddressesView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _context = new ToyShopContext();
            LoadData();
        }

        private void LoadData()
        {
            var addresses = _context.Addresses.Where(a => a.IdUser == _userId).ToList();
            _allAddresses = addresses.Select(a => new AddressDisplay
            {
                IdAddress = a.IdAddress,
                City = a.City ?? "",
                Street = a.Street ?? "",
                House = a.House ?? "",
                Apartment = a.Apartment ?? ""
            }).ToList();
            icAddresses.ItemsSource = _allAddresses;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icAddresses.ItemsSource = _allAddresses;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allAddresses
                    .Where(a => a.City.ToLower().Contains(search) || a.Street.ToLower().Contains(search))
                    .ToList();
                icAddresses.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            if (!(card.Tag is int addressId)) return;

            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedAddressId = addressId;
        }

        private AddressDisplay GetSelectedAddress()
        {
            if (_selectedAddressId == -1) return null;
            return _allAddresses.FirstOrDefault(a => a.IdAddress == _selectedAddressId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
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
                LoadData();
                MessageBox.Show("Адрес добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedAddress();
            if (selected == null)
            {
                MessageBox.Show("Выберите адрес для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var address = _context.Addresses.FirstOrDefault(a => a.IdAddress == selected.IdAddress);
            if (address == null) return;

            var dialog = new AddressDialog(address);
            if (dialog.ShowDialog() == true)
            {
                address.City = dialog.City;
                address.Street = dialog.Street;
                address.House = dialog.House;
                address.Apartment = dialog.Apartment;

                _context.Addresses.Update(address);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Адрес обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedAddress();
            if (selected == null)
            {
                MessageBox.Show("Выберите адрес для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var address = _context.Addresses.FirstOrDefault(a => a.IdAddress == selected.IdAddress);
            if (address == null) return;

            if (MessageBox.Show($"Удалить адрес {selected.FullAddress}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Addresses.Remove(address);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Адрес удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}