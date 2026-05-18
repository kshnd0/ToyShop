using System;
using System.Windows;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class EventDialog : Window
    {
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string Meaning { get; private set; }

        public EventDialog(Event eventItem = null)
        {
            InitializeComponent();

            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddDays(30);

            if (eventItem != null)
            {
                dpStartDate.SelectedDate = eventItem.StartDate;
                dpEndDate.SelectedDate = eventItem.EndDate;
                tbMeaning.Text = eventItem.Meaning;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты начала и окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            StartDate = dpStartDate.SelectedDate.Value;
            EndDate = dpEndDate.SelectedDate.Value;
            Meaning = tbMeaning.Text.Trim();

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