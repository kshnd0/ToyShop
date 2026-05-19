using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class UserDialog : Window
    {
        public string Number { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }

        // Конструктор для редактирования
        public UserDialog(User user)
        {
            InitializeComponent();

            tbNumber.Text = user.Number;
            tbEmail.Text = user.Email;

            // Выбираем роль в ComboBox
            for (int i = 0; i < cbRole.Items.Count; i++)
            {
                var item = cbRole.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == user.Role)
                {
                    cbRole.SelectedIndex = i;
                    break;
                }
            }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^8-\d{3}-\d{3}-\d{2}-\d{2}$");
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNumber.Text))
            {
                MessageBox.Show("Введите телефон!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show("Введите email!", "Ошибka", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string phone = tbNumber.Text.Trim();
            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Телефон должен быть в формате: 8-XXX-XXX-XX-XX", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string email = tbEmail.Text.Trim();
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Number = phone;
            Email = email;

            var selectedRole = cbRole.SelectedItem as ComboBoxItem;
            Role = selectedRole != null ? selectedRole.Content.ToString() : "Customer";

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