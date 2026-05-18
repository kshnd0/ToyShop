using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class UserDialog : Window
    {
        public string Name { get; private set; }
        public string Lastname { get; private set; }
        public string Patronymic { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Number { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }

        public UserDialog(User user = null)
        {
            InitializeComponent();

            if (user != null)
            {
                tbName.Text = user.Name;
                tbLastname.Text = user.Lastname;
                tbPatronymic.Text = user.Patronymic;
                tbLogin.Text = user.Login;
                pbPassword.Password = user.Password;
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
            else
            {
                cbRole.SelectedIndex = 2; // По умолчанию Customer
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Проверка формата: 8-999-999-99-99
            if (string.IsNullOrEmpty(phone)) return false;
            return Regex.IsMatch(phone, @"^8-\d{3}-\d{3}-\d{2}-\d{2}$");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // Проверка имени
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(tbLastname.Text))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка логина
            if (string.IsNullOrWhiteSpace(tbLogin.Text))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка телефона (формат 8-999-999-99-99)
            string phone = tbNumber.Text.Trim();
            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Телефон должен быть в формате: 8-999-999-99-99\nПример: 8-912-345-67-89",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка email
            string email = tbEmail.Text.Trim();
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный email!\nПример: user@mail.ru",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохраняем значения
            Name = tbName.Text.Trim();
            Lastname = tbLastname.Text.Trim();
            Patronymic = tbPatronymic.Text.Trim();
            Login = tbLogin.Text.Trim();
            Password = pbPassword.Password;
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