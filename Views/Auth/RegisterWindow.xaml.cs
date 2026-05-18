using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ToyShop.Models;

namespace ToyShop
{
    public partial class RegisterWindow : Window
    {
        private bool _isPasswordVisible = false;

        public RegisterWindow()
        {
            InitializeComponent();
            pbPassword.PasswordChanged += PbPassword_PasswordChanged;
        }

        // Проверка пароля и подсветка требований
        private void ValidateAndHighlightPassword(string password)
        {
            bool lengthOk = password.Length >= 6;
            tbLengthReq.Text = lengthOk ? "✓ Минимум 6 символов" : "✗ Минимум 6 символов";
            tbLengthReq.Foreground = lengthOk ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

            bool upperOk = Regex.IsMatch(password, "[A-Z]");
            tbUpperReq.Text = upperOk ? "✓ Минимум 1 прописная буква (A-Z)" : "✗ Минимум 1 прописная буква (A-Z)";
            tbUpperReq.Foreground = upperOk ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

            bool digitOk = Regex.IsMatch(password, "[0-9]");
            tbDigitReq.Text = digitOk ? "✓ Минимум 1 цифра (0-9)" : "✗ Минимум 1 цифра (0-9)";
            tbDigitReq.Foreground = digitOk ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

            bool specialOk = Regex.IsMatch(password, "[!@#$%^]");
            tbSpecialReq.Text = specialOk ? "✓ Минимум 1 спецсимвол (! @ # $ % ^)" : "✗ Минимум 1 спецсимвол (! @ # $ % ^)";
            tbSpecialReq.Foreground = specialOk ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = pbPassword.Password;
            ValidateAndHighlightPassword(password);

            if (_isPasswordVisible && tbVisiblePassword.Visibility == Visibility.Visible)
            {
                tbVisiblePassword.Text = password;
            }
        }

        private void TbVisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isPasswordVisible)
            {
                pbPassword.Password = tbVisiblePassword.Text;
                ValidateAndHighlightPassword(tbVisiblePassword.Text);
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                tbVisiblePassword.Text = pbPassword.Password;
                tbVisiblePassword.Visibility = Visibility.Visible;
                pbPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                pbPassword.Password = tbVisiblePassword.Text;
                pbPassword.Visibility = Visibility.Visible;
                tbVisiblePassword.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsPasswordValid(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (password.Length < 6)
            {
                errorMessage = "Пароль должен содержать минимум 6 символов!";
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                errorMessage = "Пароль должен содержать минимум 1 прописную букву (A-Z)!";
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                errorMessage = "Пароль должен содержать минимум 1 цифру (0-9)!";
                return false;
            }

            if (!Regex.IsMatch(password, "[!@#$%^]"))
            {
                errorMessage = "Пароль должен содержать минимум 1 спецсимвол из набора: ! @ # $ % ^";
                return false;
            }

            return true;
        }

        private bool IsPhoneValid(string phone, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(phone))
            {
                errorMessage = "Введите номер телефона!";
                return false;
            }

            if (!Regex.IsMatch(phone, @"^8-\d{3}-\d{3}-\d{2}-\d{2}$"))
            {
                errorMessage = "Телефон должен быть в формате: 8-XXX-XXX-XX-XX\nПример: 8-999-123-45-67";
                return false;
            }

            return true;
        }

        private bool IsEmailValid(string email, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(email))
            {
                errorMessage = "Введите email!";
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorMessage = "Введите корректный email!\nПример: user@mail.ru";
                return false;
            }

            return true;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage;

            // Очищаем предыдущую ошибку
            tbError.Text = "";

            // Проверка имени
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                tbError.Text = "Введите имя!";
                return;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(tbLastname.Text))
            {
                tbError.Text = "Введите фамилию!";
                return;
            }

            // Проверка логина
            if (string.IsNullOrWhiteSpace(tbLogin.Text))
            {
                tbError.Text = "Введите логин!";
                return;
            }

            // Получаем пароль (из видимого или скрытого поля)
            string password = _isPasswordVisible ? tbVisiblePassword.Text : pbPassword.Password;
            if (string.IsNullOrEmpty(password))
            {
                tbError.Text = "Введите пароль!";
                return;
            }

            // Проверка подтверждения пароля
            string confirmPassword = pbConfirmPassword.Password;
            if (string.IsNullOrEmpty(confirmPassword))
            {
                tbError.Text = "Повторите пароль!";
                return;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(tbNumber.Text))
            {
                tbError.Text = "Введите телефон!";
                return;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                tbError.Text = "Введите email!";
                return;
            }

            // Валидация пароля
            if (!IsPasswordValid(password, out errorMessage))
            {
                tbError.Text = errorMessage;
                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                tbError.Text = "Пароли не совпадают!";
                return;
            }

            // Валидация телефона
            if (!IsPhoneValid(tbNumber.Text.Trim(), out errorMessage))
            {
                tbError.Text = errorMessage;
                return;
            }

            // Валидация email
            if (!IsEmailValid(tbEmail.Text.Trim(), out errorMessage))
            {
                tbError.Text = errorMessage;
                return;
            }

            try
            {
                using (var context = new ToyShopContext())
                {
                    // Проверка уникальности логина
                    if (context.Users.Any(u => u.Login == tbLogin.Text.Trim()))
                    {
                        tbError.Text = "Логин уже существует!";
                        return;
                    }

                    // Проверка уникальности email
                    if (context.Users.Any(u => u.Email == tbEmail.Text.Trim()))
                    {
                        tbError.Text = "Email уже существует!";
                        return;
                    }

                    var user = new User
                    {
                        Name = tbName.Text.Trim(),
                        Lastname = tbLastname.Text.Trim(),
                        Login = tbLogin.Text.Trim(),
                        Password = password,
                        Number = tbNumber.Text.Trim(),
                        Email = tbEmail.Text.Trim(),
                        Role = "Customer"
                    };

                    context.Users.Add(user);
                    context.SaveChanges();

                    MessageBox.Show("Регистрация успешна! Теперь войдите в систему.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                tbError.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}