using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class ProvidersView : UserControl
    {
        private ToyShopContext _context;
        private List<ProviderDisplay> _allProviders;
        private Border _selectedCard;
        private int _selectedProviderId = -1;  // Добавляем поле для хранения ID

        public class ProviderDisplay
        {
            public int IdProvider { get; set; }
            public string Name { get; set; } = "";
            public string Number { get; set; } = "";
            public string Inn { get; set; } = "";
        }

        public ProvidersView()
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
            var providers = _context.Providers.ToList();
            _allProviders = providers.Select(p => new ProviderDisplay
            {
                IdProvider = p.IdProvider,
                Name = p.Name ?? "",
                Number = p.Number ?? "",
                Inn = p.Inn ?? ""
            }).ToList();
            icProviders.ItemsSource = _allProviders;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icProviders.ItemsSource = _allProviders;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allProviders
                    .Where(p => p.Name.ToLower().Contains(search) ||
                                p.Number.ToLower().Contains(search))
                    .ToList();
                icProviders.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            var providerId = (int)card.Tag;

            // Снимаем выделение с предыдущей карточки
            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            // Выделяем новую карточку
            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedProviderId = providerId;
        }

        private ProviderDisplay GetSelectedProvider()
        {
            if (_selectedProviderId == -1) return null;
            return _allProviders.FirstOrDefault(p => p.IdProvider == _selectedProviderId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProviderDialog();
            if (dialog.ShowDialog() == true)
            {
                var provider = new Provider
                {
                    Name = dialog.Name,
                    Number = dialog.Number,
                    Inn = dialog.Inn
                };
                _context.Providers.Add(provider);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Поставщик добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedProvider();
            if (selected == null)
            {
                MessageBox.Show("Выберите поставщика для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var provider = _context.Providers.FirstOrDefault(p => p.IdProvider == selected.IdProvider);
            if (provider == null) return;

            var dialog = new ProviderDialog(provider);
            if (dialog.ShowDialog() == true)
            {
                provider.Name = dialog.Name;
                provider.Number = dialog.Number;
                provider.Inn = dialog.Inn;

                _context.Providers.Update(provider);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Поставщик обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedProvider();
            if (selected == null)
            {
                MessageBox.Show("Выберите поставщика для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var provider = _context.Providers.FirstOrDefault(p => p.IdProvider == selected.IdProvider);
            if (provider == null) return;

            if (MessageBox.Show($"Удалить поставщика \"{provider.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Providers.Remove(provider);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Поставщик удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}