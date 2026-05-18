using System.Windows;
using System.Windows.Controls;

namespace ToyShop.Views.UserControls
{
    public partial class OrderDialog : Window
    {
        public string Status { get; private set; }

        public OrderDialog()
        {
            InitializeComponent();
        }

        public OrderDialog(string currentStatus)
        {
            InitializeComponent();

            for (int i = 0; i < cbStatus.Items.Count; i++)
            {
                var item = cbStatus.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == currentStatus)
                {
                    cbStatus.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var selected = cbStatus.SelectedItem as ComboBoxItem;
            if (selected != null)
            {
                Status = selected.Content.ToString();
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}