using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class OrdersView : UserControl
    {
        private ToyShopContext _context;
        private List<OrderDisplay> _allOrders;
        private Border _selectedCard;
        private int _selectedOrderId = -1;
        private int _currentUserId;
        private string _currentUserRole;

        public class OrderDisplay
        {
            public int IdOrder { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string Status { get; set; } = "";
            public string PayMethod { get; set; } = "";
            public string BuyerName { get; set; } = "";
            public string EmployeeName { get; set; } = "";
        }

        public OrdersView(int userId = 0, string userRole = "Manager")
        {
            InitializeComponent();
            _context = new ToyShopContext();
            _currentUserId = userId;
            _currentUserRole = userRole;
            LoadData();
        }

        public void SetButtonsVisibility(Visibility visibility)
        {
            btnEdit.Visibility = visibility;
        }

        public void LoadData()
        {
            var query = _context.Orders.AsQueryable();

            if (_currentUserRole == "Customer" && _currentUserId > 0)
            {
                query = query.Where(o => o.IdUser == _currentUserId);
            }

            var orders = query.ToList();
            var users = _context.Users.ToDictionary(u => u.IdUser, u => $"{u.Name} {u.Lastname}");
            var employees = _context.Employees.ToDictionary(e => e.IdEmployee, e => $"{e.Name} {e.Lastname}");

            _allOrders = orders.Select(o => new OrderDisplay
            {
                IdOrder = o.IdOrder,
                RegistrationDate = o.RegistrationDate ?? DateTime.Now,
                Status = o.Status ?? "",
                PayMethod = o.PayMethod ?? "",
                BuyerName = _currentUserRole == "Customer" ? "Вы" : (users.ContainsKey(o.IdUser) ? users[o.IdUser] : "Не указан"),
                EmployeeName = employees.ContainsKey(o.IdEmployee) ? employees[o.IdEmployee] : "Не указан"
            }).ToList();

            icOrders.ItemsSource = _allOrders;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icOrders.ItemsSource = _allOrders;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allOrders
                    .Where(o => o.IdOrder.ToString().Contains(search) ||
                                o.Status.ToLower().Contains(search))
                    .ToList();
                icOrders.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            if (!(card.Tag is int orderId)) return;

            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedOrderId = orderId;
        }

        private OrderDisplay GetSelectedOrder()
        {
            if (_selectedOrderId == -1) return null;
            return _allOrders?.FirstOrDefault(o => o.IdOrder == _selectedOrderId);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedOrder();
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var order = _context.Orders.FirstOrDefault(o => o.IdOrder == selected.IdOrder);
            if (order == null) return;

            // Простой диалог только для изменения статуса
            var dialog = new OrderDialog(order.Status);
            if (dialog.ShowDialog() == true)
            {
                order.Status = dialog.Status;
                _context.Orders.Update(order);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Статус заказа обновлён!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}