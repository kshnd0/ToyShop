
using System;
using System.Linq;
using System.Windows;
using ToyShop.Models;
using ToyShop.Views.Main;

namespace ToyShop
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            CheckConnection();
        }

        private void CheckConnection()
        {
            try
            {
                using (var context = new ToyShopContext())
                {
                    if (context.Database.CanConnect())
                        tbStatus.Text = " Подключено";
                }
            }
            catch
            {
                tbStatus.Text = " Ошибка подключения";
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                tbError.Text = "Введите логин и пароль";
                return;
            }

            try
            {
                using (var context = new ToyShopContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                    if (user != null)
                    {
                        var mainWindow = new MainWindow(user);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        tbError.Text = "Неверный логин или пароль";
                    }
                }
            }
            catch (Exception ex)
            {
                tbError.Text = ex.Message;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}