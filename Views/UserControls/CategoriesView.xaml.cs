using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToyShop.Models;
using System.IO;

namespace ToyShop.Views.UserControls
{
    public partial class CategoriesView : UserControl
    {
        private ToyShopContext _context;
        private List<Category> _allCategories;
        private Border _selectedCard;
        private int _selectedCategoryId = -1;

        public event EventHandler<int> CategorySelected;

        public CategoriesView()
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
            _allCategories = _context.Categories.ToList();
            DisplayCategories(_allCategories);
            tbCount.Text = $"Всего категорий: {_allCategories.Count}";
        }

        private void DisplayCategories(List<Category> categories)
        {
            wpCategories.Children.Clear();

            foreach (var category in categories)
            {
                var card = CreateCategoryCard(category);
                wpCategories.Children.Add(card);
            }
        }

        private Border CreateCategoryCard(Category category)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(8),
                Padding = new Thickness(15),
                Width = 180,
                Height = 150,
                BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202)),
                BorderThickness = new Thickness(1),
                Tag = category,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            border.MouseEnter += (s, e) =>
            {
                border.Background = new SolidColorBrush(Color.FromRgb(209, 238, 252));
            };

            border.MouseLeave += (s, e) =>
            {
                border.Background = new SolidColorBrush(Colors.White);
            };

            border.MouseLeftButtonUp += (s, e) =>
            {
                CategorySelected?.Invoke(this, category.IdCategory);

                if (_selectedCard != null)
                {
                    _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                    _selectedCard.BorderThickness = new Thickness(1);
                }
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                border.BorderThickness = new Thickness(2);
                _selectedCard = border;
                _selectedCategoryId = category.IdCategory;
            };

            var stackPanel = new StackPanel();

            // КАРТИНКА КАТЕГОРИИ (рабочий путь)
            var image = new Image
            {
                Width = 80,
                Height = 80,
                Margin = new Thickness(0, 5, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Stretch = Stretch.UniformToFill
            };

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = System.IO.Path.Combine(basePath, "Images", "Categories", $"{category.IdCategory}.jpg");

            try
            {
                if (File.Exists(imagePath))
                {
                    image.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    image.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                image.Visibility = Visibility.Collapsed;
            }
            stackPanel.Children.Add(image);

            // НАЗВАНИЕ КАТЕГОРИИ
            var nameText = new TextBlock
            {
                Text = category.Name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
            stackPanel.Children.Add(nameText);

            // КОЛИЧЕСТВО ТОВАРОВ
            var countText = new TextBlock
            {
                Text = $"{_context.Products.Count(p => p.IdCategory == category.IdCategory)} товаров",
                FontSize = 11,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0)
            };
            stackPanel.Children.Add(countText);

            border.Child = stackPanel;
            return border;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                DisplayCategories(_allCategories);
                tbCount.Text = $"Всего категорий: {_allCategories.Count}";
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allCategories
                    .Where(c => c.Name.ToLower().Contains(search))
                    .ToList();
                DisplayCategories(filtered);
                tbCount.Text = $"Найдено: {filtered.Count} из {_allCategories.Count}";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog("Введите название категории", "Добавление категории");
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.InputText))
            {
                var category = new Category { Name = dialog.InputText };
                _context.Categories.Add(category);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Категория добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategoryId == -1)
            {
                MessageBox.Show("Выберите категорию для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var category = _context.Categories.FirstOrDefault(c => c.IdCategory == _selectedCategoryId);
            if (category == null) return;

            var dialog = new InputDialog("Введите новое название", "Редактирование категории", category.Name);
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.InputText))
            {
                category.Name = dialog.InputText;
                _context.Categories.Update(category);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Категория обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategoryId == -1)
            {
                MessageBox.Show("Выберите категорию для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var category = _context.Categories.FirstOrDefault(c => c.IdCategory == _selectedCategoryId);
            if (category == null) return;

            var productsCount = _context.Products.Count(p => p.IdCategory == category.IdCategory);
            if (productsCount > 0)
            {
                MessageBox.Show($"Невозможно удалить категорию \"{category.Name}\", так как в ней есть товары!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить категорию \"{category.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Категория удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}