using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class EmployeesView : UserControl
    {
        private ToyShopContext _context;
        private List<EmployeeDisplay> _allEmployees;
        private Border _selectedCard;
        private int _selectedEmployeeId = -1;  // Добавляем поле для хранения ID

        public class EmployeeDisplay
        {
            public int IdEmployee { get; set; }
            public string Name { get; set; } = "";
            public string Lastname { get; set; } = "";
            public string Patronymic { get; set; } = "";
            public string FullName => $"{Lastname} {Name} {Patronymic}".Trim();
            public string Number { get; set; } = "";
            public string Email { get; set; } = "";
            public string WarehouseName { get; set; } = "";
        }

        public EmployeesView()
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
            var employees = _context.Employees.ToList();
            var warehouses = _context.Warehouses.ToDictionary(w => w.IdWarehouse, w => w.Name);

            _allEmployees = employees.Select(e => new EmployeeDisplay
            {
                IdEmployee = e.IdEmployee,
                Name = e.Name ?? "",
                Lastname = e.Lastname ?? "",
                Patronymic = e.Patronymic ?? "",
                Number = e.Number ?? "",
                Email = e.Email ?? "",
                WarehouseName = e.IdWarehouse != null && warehouses.ContainsKey(e.IdWarehouse.Value)
                    ? warehouses[e.IdWarehouse.Value] : "Не назначен"
            }).ToList();
            icEmployees.ItemsSource = _allEmployees;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icEmployees.ItemsSource = _allEmployees;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allEmployees
                    .Where(e => e.FullName.ToLower().Contains(search) ||
                                e.Email.ToLower().Contains(search))
                    .ToList();
                icEmployees.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            var employeeId = (int)card.Tag;

            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedEmployeeId = employeeId;
        }

        private EmployeeDisplay GetSelectedEmployee()
        {
            if (_selectedEmployeeId == -1) return null;
            return _allEmployees.FirstOrDefault(e => e.IdEmployee == _selectedEmployeeId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var warehouses = _context.Warehouses.ToList();
            var dialog = new EmployeeDialog(warehouses);
            if (dialog.ShowDialog() == true)
            {
                var employee = new Employee
                {
                    Name = dialog.Name,
                    Lastname = dialog.Lastname,
                    Patronymic = dialog.Patronymic,
                    Number = dialog.Number,
                    Email = dialog.Email,
                    IdWarehouse = dialog.SelectedWarehouseId
                };
                _context.Employees.Add(employee);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Сотрудник добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEmployee();
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = _context.Employees.FirstOrDefault(emp => emp.IdEmployee == selected.IdEmployee);
            if (employee == null) return;

            var warehouses = _context.Warehouses.ToList();
            var dialog = new EmployeeDialog(warehouses, employee);
            if (dialog.ShowDialog() == true)
            {
                employee.Name = dialog.Name;
                employee.Lastname = dialog.Lastname;
                employee.Patronymic = dialog.Patronymic;
                employee.Number = dialog.Number;
                employee.Email = dialog.Email;
                employee.IdWarehouse = dialog.SelectedWarehouseId;

                _context.Employees.Update(employee);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Сотрудник обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEmployee();
            if (selected == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = _context.Employees.FirstOrDefault(emp => emp.IdEmployee == selected.IdEmployee);
            if (employee == null) return;

            if (MessageBox.Show($"Удалить сотрудника {selected.FullName}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Сотрудник удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}