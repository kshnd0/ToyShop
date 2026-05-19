using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class EventAddDialog : Window
    {
        public string EventName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<SelectedCategory> SelectedCategories { get; private set; }

        public class SelectedCategory
        {
            public int IdCategory { get; set; }
            public decimal DiscountPercent { get; set; }
        }

        public EventAddDialog()
        {
            InitializeComponent();
            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddDays(7);
            LoadCategories(null);
        }

        public EventAddDialog(Event eventItem, List<CategoryEvent> currentCategoryEvents)
        {
            InitializeComponent();

            tbEventName.Text = eventItem.Meaning;
            dpStartDate.SelectedDate = eventItem.StartDate;
            dpEndDate.SelectedDate = eventItem.EndDate;
            LoadCategories(currentCategoryEvents);
        }

        private void LoadCategories(List<CategoryEvent> currentCategoryEvents)
        {
            spCategories.Children.Clear();

            using (var context = new ToyShopContext())
            {
                var categories = context.Categories.ToList();

                foreach (var cat in categories)
                {
                    decimal currentDiscount = 0;
                    if (currentCategoryEvents != null)
                    {
                        var existing = currentCategoryEvents.FirstOrDefault(ce => ce.IdCategory == cat.IdCategory);
                        if (existing != null)
                        {
                            currentDiscount = existing.DiscountPercent;
                        }
                    }

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var nameText = new TextBlock
                    {
                        Text = cat.Name,
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    Grid.SetColumn(nameText, 0);

                    var discountPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var discountText = new TextBlock
                    {
                        Text = "Скидка:",
                        FontSize = 12,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 5, 0)
                    };

                    var discountBox = new TextBox
                    {
                        Width = 60,
                        Height = 30,
                        TextAlignment = TextAlignment.Center,
                        Text = currentDiscount.ToString(),
                        Tag = cat.IdCategory
                    };

                    var percentText = new TextBlock
                    {
                        Text = "%",
                        FontSize = 12,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0)
                    };

                    discountPanel.Children.Add(discountText);
                    discountPanel.Children.Add(discountBox);
                    discountPanel.Children.Add(percentText);
                    Grid.SetColumn(discountPanel, 1);

                    grid.Children.Add(nameText);
                    grid.Children.Add(discountPanel);

                    var border = new Border
                    {
                        Background = System.Windows.Media.Brushes.White,
                        CornerRadius = new CornerRadius(8),
                        Margin = new Thickness(0, 0, 0, 5),
                        Padding = new Thickness(10),
                        BorderBrush = System.Windows.Media.Brushes.LightGray,
                        BorderThickness = new Thickness(1),
                        Child = grid
                    };

                    spCategories.Children.Add(border);
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbEventName.Text))
            {
                MessageBox.Show("Введите название акции!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EventName = tbEventName.Text.Trim();
            StartDate = dpStartDate.SelectedDate.Value;
            EndDate = dpEndDate.SelectedDate.Value;

            SelectedCategories = new List<SelectedCategory>();

            foreach (UIElement element in spCategories.Children)
            {
                if (element is Border border && border.Child is Grid grid)
                {
                    TextBox foundBox = null;
                    int categoryId = 0;

                    foreach (UIElement child in grid.Children)
                    {
                        if (child is StackPanel panel)
                        {
                            foreach (UIElement panelChild in panel.Children)
                            {
                                if (panelChild is TextBox tb)
                                {
                                    foundBox = tb;
                                    categoryId = (int)tb.Tag;
                                }
                            }
                        }
                    }

                    if (foundBox != null && !string.IsNullOrEmpty(foundBox.Text) && decimal.TryParse(foundBox.Text, out decimal discount) && discount > 0)
                    {
                        SelectedCategories.Add(new SelectedCategory
                        {
                            IdCategory = categoryId,
                            DiscountPercent = discount
                        });
                    }
                }
            }

            if (SelectedCategories.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну категорию со скидкой!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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