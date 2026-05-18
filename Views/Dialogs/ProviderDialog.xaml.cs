using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class ProviderDialog : Window
    {
        public string Name { get; private set; }
        public string Number { get; private set; }
        public string Inn { get; private set; }

        public ProviderDialog(Provider provider = null)
        {
            InitializeComponent();

            if (provider != null)
            {
                tbName.Text = provider.Name;
                tbNumber.Text = provider.Number;
                tbInn.Text = provider.Inn;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbNumber.Text))
            {
                MessageBox.Show("Введите телефон поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Name = tbName.Text.Trim();
            Number = tbNumber.Text.Trim();
            Inn = tbInn.Text.Trim();

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