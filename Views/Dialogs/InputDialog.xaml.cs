using System.Windows;

namespace ToyShop.Views.UserControls
{
    public partial class InputDialog : Window
    {
        public string InputText { get; private set; }

        public InputDialog(string message, string title, string defaultValue = "")
        {
            InitializeComponent();
            Title = title;
            tbMessage.Text = message;
            tbInput.Text = defaultValue;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            InputText = tbInput.Text.Trim();
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