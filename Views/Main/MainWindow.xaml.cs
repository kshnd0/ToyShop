using System.Linq;
using System.Windows;
using ToyShop.Models;

using ToyShop.Views.Customer;
using ToyShop.Views.UserControls;

namespace ToyShop.Views.Main
{
    public partial class MainWindow : Window
    {
        private User currentUser;
        private CategoriesView _categoriesView;
        private ProductsView _productsView;
        private OrdersView _ordersView;
        private AddressesView _addressesView;
        private EmployeesView _employeesView;
        private ProvidersView _providersView;
        private WarehousesView _warehousesView;
        private StockView _stockView;
        private UsersView _usersView;

        public MainWindow(User user)
        {
            InitializeComponent();
            currentUser = user;

            tbUserInfo.Text = $"{user.Name} {user.Lastname} | Роль: {user.Role}";
            tbWelcome.Text = $"Добро пожаловать, {user.Name}!";

            _categoriesView = new CategoriesView();
            _productsView = new ProductsView(currentUser.IdUser, currentUser.Role);
            _ordersView = new OrdersView(currentUser.IdUser, currentUser.Role);
            _addressesView = new AddressesView(currentUser.IdUser);
            _employeesView = new EmployeesView();
            _providersView = new ProvidersView();
            _warehousesView = new WarehousesView();
            _stockView = new StockView();
            _usersView = new UsersView();

            // ПОДПИСКА НА СОБЫТИЕ ВЫБОРА КАТЕГОРИИ
            _categoriesView.CategorySelected += CategoriesView_CategorySelected;

            CategoriesContentControl.Content = _categoriesView;
            ProductsContentControl.Content = _productsView;
            OrdersContentControl.Content = _ordersView;
            AddressesContentControl.Content = _addressesView;
            EmployeesContentControl.Content = _employeesView;
            ProvidersContentControl.Content = _providersView;
            WarehousesContentControl.Content = _warehousesView;
            StockContentControl.Content = _stockView;
            UsersContentControl.Content = _usersView;

            ConfigureTabsByRole();
        }

        // ОБРАБОТЧИК ВЫБОРА КАТЕГОРИИ
        private void CategoriesView_CategorySelected(object sender, int categoryId)
        {
            // Переключаемся на вкладку "Товары"
            MainTab.SelectedItem = tabProducts;

            // Фильтруем товары по выбранной категории
            if (ProductsContentControl.Content is ProductsView productsView)
            {
                productsView.FilterByCategory(categoryId);
                tbStatus.Text = $"Показаны товары категории";
            }
        }

        public void RefreshOrders()
        {
            if (OrdersContentControl.Content is OrdersView ordersView)
            {
                ordersView.LoadData();
                tbStatus.Text = "✓ Заказы обновлены";
            }
        }

        private void ConfigureTabsByRole()
        {
            // Скрываем всё кроме категорий, товаров, заказов и главной
            tabEmployee.Visibility = Visibility.Collapsed;
            tabProvider.Visibility = Visibility.Collapsed;
            tabWarehouse.Visibility = Visibility.Collapsed;
            tabStock.Visibility = Visibility.Collapsed;
            tabUser.Visibility = Visibility.Collapsed;
            tabAddresses.Visibility = Visibility.Collapsed;
            btnCart.Visibility = Visibility.Collapsed;

            if (currentUser.Role == "Admin")
            {
                // Админ видит всё (кроме акций)
                tabEmployee.Visibility = Visibility.Visible;
                tabProvider.Visibility = Visibility.Visible;
                tabWarehouse.Visibility = Visibility.Visible;
                tabStock.Visibility = Visibility.Visible;
                tabUser.Visibility = Visibility.Visible;
                tabAddresses.Visibility = Visibility.Collapsed;
                btnCart.Visibility = Visibility.Collapsed;

                _categoriesView.SetButtonsVisibility(Visibility.Visible);
                _productsView.SetButtonsVisibility(Visibility.Visible);
                _ordersView.SetButtonsVisibility(Visibility.Visible);
            }
            else if (currentUser.Role == "Manager")
            {
                // Менеджер видит только категории, товары, заказы
                tabEmployee.Visibility = Visibility.Collapsed;
                tabProvider.Visibility = Visibility.Collapsed;
                tabWarehouse.Visibility = Visibility.Collapsed;
                tabStock.Visibility = Visibility.Collapsed;
                tabUser.Visibility = Visibility.Collapsed;
                tabAddresses.Visibility = Visibility.Collapsed;
                btnCart.Visibility = Visibility.Collapsed;

                _categoriesView.SetButtonsVisibility(Visibility.Visible);
                _productsView.SetButtonsVisibility(Visibility.Visible);
                _ordersView.SetButtonsVisibility(Visibility.Visible);
            }
            else if (currentUser.Role == "Customer")
            {
                // Покупатель видит только категории, товары, свои заказы, адреса и корзину
                tabEmployee.Visibility = Visibility.Collapsed;
                tabProvider.Visibility = Visibility.Collapsed;
                tabWarehouse.Visibility = Visibility.Collapsed;
                tabStock.Visibility = Visibility.Collapsed;
                tabUser.Visibility = Visibility.Collapsed;
                tabAddresses.Visibility = Visibility.Visible;
                btnCart.Visibility = Visibility.Visible;

                _categoriesView.SetButtonsVisibility(Visibility.Collapsed);
                _productsView.SetButtonsVisibility(Visibility.Collapsed);
                _ordersView.SetButtonsVisibility(Visibility.Collapsed);
            }
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser.Role == "Customer")
            {
                var cartWindow = new CartWindow(currentUser.IdUser);
                cartWindow.Owner = this;
                cartWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Только покупатели имеют доступ к корзине!", "Доступ запрещён",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}