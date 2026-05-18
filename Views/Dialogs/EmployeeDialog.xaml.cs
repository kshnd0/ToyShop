using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class EmployeeDialog : Window
    {
        public string Name { get; private set; }
        public string Lastname { get; private set; }
        public string Patronymic { get; private set; }
        public string Number { get; private set; }
        public string Email { get; private set; }
        public int? SelectedWarehouseId { get; private set; }

        public EmployeeDialog(List<Warehouse> warehouses, Employee employee = null)
        {
            InitializeComponent();

            cbWarehouse.ItemsSource = warehouses;

            if (employee != null)
            {
                tbName.Text = employee.Name;
                tbLastname.Text = employee.Lastname;
                tbPatronymic.Text = employee.Patronymic;
                tbNumber.Text = employee.Number;
                tbEmail.Text = employee.Email;
                cbWarehouse.SelectedValue = employee.IdWarehouse;
            }

            if (warehouses.Any()) cbWarehouse.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text) ||
                string.IsNullOrWhiteSpace(tbLastname.Text) ||
                string.IsNullOrWhiteSpace(tbNumber.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Name = tbName.Text.Trim();
            Lastname = tbLastname.Text.Trim();
            Patronymic = tbPatronymic.Text.Trim();
            Number = tbNumber.Text.Trim();
            Email = tbEmail.Text.Trim();
            SelectedWarehouseId = cbWarehouse.SelectedValue as int?;

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