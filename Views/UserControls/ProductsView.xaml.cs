using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class ProductsView : UserControl
    {
        private ToyShopContext _context;
        private List<Product> _allProducts;
        private int _currentUserId;
        private string _currentUserRole;
        private int _filterCategoryId = 0;
        private Border _selectedCard;

        public ProductsView(int userId = 0, string userRole = "Customer")
        {
            InitializeComponent();
            _currentUserId = userId;
            _currentUserRole = userRole;
            _context = new ToyShopContext();
            LoadData();
        }

        public void SetButtonsVisibility(Visibility visibility)
        {
            btnAdd.Visibility = visibility;
            btnEdit.Visibility = visibility;
            btnDelete.Visibility = visibility;
            btnExport.Visibility = visibility;
        }

        public void ShowResetButton(bool show)
        {
            btnResetFilter.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public void FilterByCategory(int categoryId)
        {
            _filterCategoryId = categoryId;
            ShowResetButton(true);

            if (categoryId == 0)
            {
                DisplayProducts(_allProducts);
                tbCount.Text = $"Всего товаров: {_allProducts.Count}";
                ShowResetButton(false);
            }
            else
            {
                var filtered = _allProducts.Where(p => p.IdCategory == categoryId).ToList();
                DisplayProducts(filtered);
                var categoryName = _context.Categories.FirstOrDefault(c => c.IdCategory == categoryId)?.Name ?? "Категория";
                tbCount.Text = $"Товары в категории \"{categoryName}\": {filtered.Count}";
            }
        }

        public void ResetFilter()
        {
            _filterCategoryId = 0;
            DisplayProducts(_allProducts);
            tbCount.Text = $"Всего товаров: {_allProducts.Count}";
            ShowResetButton(false);
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
        }

        private void LoadData()
        {
            _allProducts = _context.Products.ToList();
            DisplayProducts(_allProducts);
            tbCount.Text = $"Всего товаров: {_allProducts.Count}";
        }

        private void DisplayProducts(List<Product> products)
        {
            wpProducts.Children.Clear();

            foreach (var product in products)
            {
                var card = CreateProductCard(product);
                wpProducts.Children.Add(card);
            }
        }

        private Border CreateProductCard(Product product)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(225, 225, 225)),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Width = 220,
                BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202)),
                BorderThickness = new Thickness(2),
                Tag = product
            };

            var stackPanel = new StackPanel();

            var image = new Image
            {
                Width = 150,
                Height = 150,
                Margin = new Thickness(0, 0, 0, 10),
                Stretch = Stretch.UniformToFill
            };

            string imagePath = $"/Images/Products/{product.IdProduct}.jpg";

            try
            {
                var uri = new Uri(imagePath, UriKind.Relative);
                image.Source = new BitmapImage(uri);
            }
            catch
            {
                image.Visibility = Visibility.Collapsed;
            }
            stackPanel.Children.Add(image);

            var nameText = new TextBlock
            {
                Text = product.Name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            stackPanel.Children.Add(nameText);

            var costText = new TextBlock
            {
                Text = $"{Math.Round(product.Cost, 2)} руб.",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(181, 213, 202)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };
            stackPanel.Children.Add(costText);

            var manufacturerText = new TextBlock
            {
                Text = product.Manufacturer,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(manufacturerText);

            if (_currentUserRole == "Customer" && _currentUserId > 0)
            {
                var addToCartButton = new Button
                {
                    Content = "В корзину",
                    Background = new SolidColorBrush(Color.FromRgb(181, 213, 202)),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(0, 10, 0, 0),
                    Height = 30,
                    Cursor = Cursors.Hand,
                    FontSize = 12,
                    Tag = product
                };
                addToCartButton.Click += AddToCart_Click;
                stackPanel.Children.Add(addToCartButton);
            }

            border.Child = stackPanel;

            border.MouseLeftButtonUp += (s, e) =>
            {
                if (_selectedCard != null)
                {
                    _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                    _selectedCard.BorderThickness = new Thickness(2);
                }
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                border.BorderThickness = new Thickness(3);
                _selectedCard = border;
            };

            return border;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button.Tag as Product;

            try
            {
                var basket = _context.Baskets.FirstOrDefault(b => b.IdUser == _currentUserId);
                if (basket == null)
                {
                    basket = new Basket { IdUser = _currentUserId };
                    _context.Baskets.Add(basket);
                    _context.SaveChanges();
                }

                var basketProduct = _context.BasketProducts
                    .FirstOrDefault(bp => bp.IdBasket == basket.IdBasket && bp.IdProduct == product.IdProduct);

                if (basketProduct != null)
                {
                    basketProduct.Amount++;
                }
                else
                {
                    basketProduct = new BasketProduct
                    {
                        IdBasket = basket.IdBasket,
                        IdProduct = product.IdProduct,
                        Amount = 1
                    };
                    _context.BasketProducts.Add(basketProduct);
                }

                _context.SaveChanges();
                MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину!", "Корзина",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Product GetSelectedProduct()
        {
            return _selectedCard?.Tag as Product;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                if (_filterCategoryId == 0)
                {
                    DisplayProducts(_allProducts);
                    tbCount.Text = $"Всего товаров: {_allProducts.Count}";
                }
                else
                {
                    var filtered = _allProducts.Where(p => p.IdCategory == _filterCategoryId).ToList();
                    DisplayProducts(filtered);
                    var categoryName = _context.Categories.FirstOrDefault(c => c.IdCategory == _filterCategoryId)?.Name ?? "Категория";
                    tbCount.Text = $"Товары в категории \"{categoryName}\": {filtered.Count}";
                }
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var baseProducts = _filterCategoryId == 0 ? _allProducts : _allProducts.Where(p => p.IdCategory == _filterCategoryId).ToList();
                var filtered = baseProducts
                    .Where(p => p.Name.ToLower().Contains(search) || p.Manufacturer.ToLower().Contains(search))
                    .ToList();
                DisplayProducts(filtered);
                tbCount.Text = $"Найдено: {filtered.Count}";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var categories = _context.Categories.ToList();
            var providers = _context.Providers.ToList();

            var dialog = new ProductDialog(categories, providers);
            if (dialog.ShowDialog() == true)
            {
                var product = new Product
                {
                    Name = dialog.ProductName,
                    Cost = dialog.ProductCost,
                    Manufacturer = dialog.Manufacturer,
                    IdCategory = dialog.SelectedCategoryId,
                    IdProvider = dialog.SelectedProviderId
                };

                _context.Products.Add(product);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Товар добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedProduct();
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var categories = _context.Categories.ToList();
            var providers = _context.Providers.ToList();

            var dialog = new ProductDialog(categories, providers, selected);
            if (dialog.ShowDialog() == true)
            {
                selected.Name = dialog.ProductName;
                selected.Cost = dialog.ProductCost;
                selected.Manufacturer = dialog.Manufacturer;
                selected.IdCategory = dialog.SelectedCategoryId;
                selected.IdProvider = dialog.SelectedProviderId;

                _context.Products.Update(selected);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Товар обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedProduct();
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить товар \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Products.Remove(selected);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Товар удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Товары_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Товары");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Название";
                    worksheet.Cells[1, 3].Value = "Цена";
                    worksheet.Cells[1, 4].Value = "Производитель";

                    for (int i = 0; i < _allProducts.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = _allProducts[i].IdProduct;
                        worksheet.Cells[i + 2, 2].Value = _allProducts[i].Name;
                        worksheet.Cells[i + 2, 3].Value = Math.Round(_allProducts[i].Cost, 2);
                        worksheet.Cells[i + 2, 4].Value = _allProducts[i].Manufacturer;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    package.SaveAs(new FileInfo(saveDialog.FileName));
                }

                MessageBox.Show("Экспорт завершён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}