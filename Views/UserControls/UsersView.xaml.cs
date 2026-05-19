using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class UsersView : UserControl
    {
        private ToyShopContext _context;
        private List<UserDisplay> _allUsers;
        private Border _selectedCard;
        private int _selectedUserId = -1;

        public class UserDisplay
        {
            public int IdUser { get; set; }
            public string Name { get; set; } = "";
            public string Lastname { get; set; } = "";
            public string Patronymic { get; set; } = "";
            public string FullName => $"{Lastname} {Name} {Patronymic}".Trim();
            public string Login { get; set; } = "";
            public string Email { get; set; } = "";
            public string Number { get; set; } = "";
            public string Role { get; set; } = "";
        }

        public UsersView()
        {
            InitializeComponent();
            _context = new ToyShopContext();
            LoadData();
        }

        public void SetButtonsVisibility(Visibility visibility)
        {
            btnEdit.Visibility = visibility;
        }

        private void LoadData()
        {
            try
            {
                var users = _context.Users.ToList();
                _allUsers = users.Select(u => new UserDisplay
                {
                    IdUser = u.IdUser,
                    Name = u.Name ?? "",
                    Lastname = u.Lastname ?? "",
                    Patronymic = u.Patronymic ?? "",
                    Login = u.Login ?? "",
                    Email = u.Email ?? "",
                    Number = u.Number ?? "",
                    Role = u.Role ?? "Customer"
                }).ToList();
                icUsers.ItemsSource = _allUsers;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icUsers.ItemsSource = _allUsers;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allUsers
                    .Where(u => u.FullName.ToLower().Contains(search) ||
                                u.Login.ToLower().Contains(search) ||
                                u.Email.ToLower().Contains(search))
                    .ToList();
                icUsers.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var card = sender as Border;
                if (card == null) return;

                if (!(card.Tag is int userId)) return;

                if (_selectedCard != null)
                {
                    _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                    _selectedCard.BorderThickness = new Thickness(1);
                }

                card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                card.BorderThickness = new Thickness(2);
                _selectedCard = card;
                _selectedUserId = userId;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка выбора: {ex.Message}");
            }
        }

        private UserDisplay GetSelectedUser()
        {
            if (_selectedUserId == -1) return null;
            return _allUsers?.FirstOrDefault(u => u.IdUser == _selectedUserId);
        }

        // Кнопка редактирования (только изменение телефона, email и роли)
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.IdUser == selected.IdUser);
            if (user == null) return;

            var dialog = new UserDialog(user);
            if (dialog.ShowDialog() == true)
            {
                user.Number = dialog.Number;
                user.Email = dialog.Email;
                user.Role = dialog.Role;

                _context.Users.Update(user);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Пользователь обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}