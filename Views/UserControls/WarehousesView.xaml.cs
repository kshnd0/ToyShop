using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;
using OfficeOpenXml;
using System.IO;

namespace ToyShop.Views.UserControls
{
    public partial class WarehousesView : UserControl
    {
        private ToyShopContext _context;
        private List<WarehouseDisplay> _allWarehouses;
        private Border _selectedCard;
        private int _selectedWarehouseId = -1;

        public class WarehouseDisplay
        {
            public int IdWarehouse { get; set; }
            public string Name { get; set; } = "";
            public string City { get; set; } = "";
            public string Street { get; set; } = "";
            public string House { get; set; } = "";
        }

        public WarehousesView()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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

        private void LoadData()
        {
            var warehouses = _context.Warehouses.ToList();
            _allWarehouses = warehouses.Select(w => new WarehouseDisplay
            {
                IdWarehouse = w.IdWarehouse,
                Name = w.Name ?? "",
                City = w.City ?? "",
                Street = w.Street ?? "",
                House = w.House ?? ""
            }).ToList();
            icWarehouses.ItemsSource = _allWarehouses;
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icWarehouses.ItemsSource = _allWarehouses;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allWarehouses
                    .Where(w => w.Name.ToLower().Contains(search) ||
                                w.City.ToLower().Contains(search))
                    .ToList();
                icWarehouses.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var card = sender as Border;
            if (card == null) return;

            var warehouseId = (int)card.Tag;

            if (_selectedCard != null)
            {
                _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                _selectedCard.BorderThickness = new Thickness(1);
            }

            card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
            card.BorderThickness = new Thickness(2);
            _selectedCard = card;
            _selectedWarehouseId = warehouseId;
        }

        private WarehouseDisplay GetSelectedWarehouse()
        {
            if (_selectedWarehouseId == -1) return null;
            return _allWarehouses.FirstOrDefault(w => w.IdWarehouse == _selectedWarehouseId);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WarehouseDialog();
            if (dialog.ShowDialog() == true)
            {
                var warehouse = new Warehouse
                {
                    Name = dialog.Name,
                    City = dialog.City,
                    Street = dialog.Street,
                    House = dialog.House
                };
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Склад добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedWarehouse();
            if (selected == null)
            {
                MessageBox.Show("Выберите склад для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var warehouse = _context.Warehouses.FirstOrDefault(w => w.IdWarehouse == selected.IdWarehouse);
            if (warehouse == null) return;

            var dialog = new WarehouseDialog(warehouse);
            if (dialog.ShowDialog() == true)
            {
                warehouse.Name = dialog.Name;
                warehouse.City = dialog.City;
                warehouse.Street = dialog.Street;
                warehouse.House = dialog.House;

                _context.Warehouses.Update(warehouse);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Склад обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedWarehouse();
            if (selected == null)
            {
                MessageBox.Show("Выберите склад для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var warehouse = _context.Warehouses.FirstOrDefault(w => w.IdWarehouse == selected.IdWarehouse);
            if (warehouse == null) return;

            var hasEmployees = _context.Employees.Any(emp => emp.IdWarehouse == warehouse.IdWarehouse);
            if (hasEmployees)
            {
                MessageBox.Show($"Невозможно удалить склад \"{warehouse.Name}\", так как на нём есть сотрудники!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить склад \"{warehouse.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Warehouses.Remove(warehouse);
                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Склад удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"Склады_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Склады");

                    
                    worksheet.Cells[1, 1].Value = "Название";
                    worksheet.Cells[1, 2].Value = "Город";
                    worksheet.Cells[1, 3].Value = "Улица";
                    worksheet.Cells[1, 4].Value = "Дом";

                   
                    using (var range = worksheet.Cells[1, 1, 1, 4])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    
                    for (int i = 0; i < _allWarehouses.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = _allWarehouses[i].Name;
                        worksheet.Cells[i + 2, 2].Value = _allWarehouses[i].City;
                        worksheet.Cells[i + 2, 3].Value = _allWarehouses[i].Street;
                        worksheet.Cells[i + 2, 4].Value = _allWarehouses[i].House;

                        
                        using (var range = worksheet.Cells[i + 2, 1, i + 2, 4])
                        {
                            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }

                    
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                   
                    worksheet.Cells[1, 1, 1, 4].AutoFilter = true;

                    package.SaveAs(new FileInfo(saveDialog.FileName));
                }

                MessageBox.Show("Экспорт завершён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}