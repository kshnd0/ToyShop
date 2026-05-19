using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using ToyShop.Models;
using System.Threading.Tasks;

namespace ToyShop.Views.UserControls
{
    public partial class EventsView : UserControl
    {
        private ToyShopContext _context;
        private List<EventDisplay> _allEvents;
        private Border _selectedCard;
        private int _selectedEventId = -1;
        private string _currentUserRole = "Customer";

        public class EventDisplay
        {
            public int IdEvent { get; set; }
            public string EventName { get; set; } = "";
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string CategoriesInfo { get; set; } = "";
        }

        public EventsView(string userRole = "Customer")
        {
            InitializeComponent();
            _currentUserRole = userRole;
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
            var events = _context.Events.ToList();
            var categoryEvents = _context.CategoryEvents
                .Include(ce => ce.IdCategoryNavigation)
                .ToList();

            _allEvents = new List<EventDisplay>();

            foreach (var ev in events)
            {
                var relatedCategories = categoryEvents
                    .Where(ce => ce.IdEvent == ev.IdEvent)
                    .Select(ce => $"{ce.IdCategoryNavigation?.Name} ({ce.DiscountPercent}%)")
                    .ToList();

                string categoriesInfo = relatedCategories.Count > 0
                    ? "Категории: " + string.Join(", ", relatedCategories)
                    : "Нет категорий";

                var eventDisplay = new EventDisplay
                {
                    IdEvent = ev.IdEvent,
                    EventName = ev.Meaning ?? "Акция",
                    StartDate = ev.StartDate ?? DateTime.Now,
                    EndDate = ev.EndDate ?? DateTime.Now,
                    CategoriesInfo = categoriesInfo
                };

                _allEvents.Add(eventDisplay);
            }

            icEvents.ItemsSource = _allEvents;
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var card = sender as Border;
                if (card == null) return;

                if (!(card.Tag is int eventId)) return;

                if (_selectedCard != null)
                {
                    _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                    _selectedCard.BorderThickness = new Thickness(1);
                }

                card.BorderBrush = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                card.BorderThickness = new Thickness(2);
                _selectedCard = card;
                _selectedEventId = eventId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выбора: {ex.Message}");
            }
        }

        private EventDisplay GetSelectedEvent()
        {
            if (_selectedEventId == -1) return null;
            return _allEvents?.FirstOrDefault(e => e.IdEvent == _selectedEventId);
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EventAddDialog();
            if (dialog.ShowDialog() == true)
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var newEvent = new Event
                        {
                            StartDate = dialog.StartDate,
                            EndDate = dialog.EndDate,
                            Meaning = dialog.EventName
                        };
                        _context.Events.Add(newEvent);
                        _context.SaveChanges();

                        foreach (var cat in dialog.SelectedCategories)
                        {
                            var categoryEvent = new CategoryEvent
                            {
                                IdEvent = newEvent.IdEvent,
                                IdCategory = cat.IdCategory,
                                DiscountPercent = cat.DiscountPercent
                            };
                            _context.CategoryEvents.Add(categoryEvent);
                        }
                        _context.SaveChanges();
                        LoadData();
                    });
                });
                MessageBox.Show("Акция добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEvent();
            if (selected == null)
            {
                MessageBox.Show("Выберите акцию для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var eventItem = _context.Events.FirstOrDefault(ev => ev.IdEvent == selected.IdEvent);
            if (eventItem == null) return;

            var currentCategoryEvents = _context.CategoryEvents
                .Where(ce => ce.IdEvent == selected.IdEvent)
                .ToList();

            var dialog = new EventAddDialog(eventItem, currentCategoryEvents);
            if (dialog.ShowDialog() == true)
            {
                eventItem.StartDate = dialog.StartDate;
                eventItem.EndDate = dialog.EndDate;
                eventItem.Meaning = dialog.EventName;

                var oldLinks = _context.CategoryEvents.Where(ce => ce.IdEvent == selected.IdEvent);
                _context.CategoryEvents.RemoveRange(oldLinks);

                foreach (var cat in dialog.SelectedCategories)
                {
                    var categoryEvent = new CategoryEvent
                    {
                        IdEvent = selected.IdEvent,
                        IdCategory = cat.IdCategory,
                        DiscountPercent = cat.DiscountPercent
                    };
                    _context.CategoryEvents.Add(categoryEvent);
                }

                await _context.SaveChangesAsync();
                LoadData();
                MessageBox.Show("Акция обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEvent();
            if (selected == null)
            {
                MessageBox.Show("Выберите акцию для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить акцию \"{selected.EventName}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var links = _context.CategoryEvents.Where(ce => ce.IdEvent == selected.IdEvent);
                _context.CategoryEvents.RemoveRange(links);

                var eventItem = _context.Events.FirstOrDefault(ev => ev.IdEvent == selected.IdEvent);
                if (eventItem != null)
                {
                    _context.Events.Remove(eventItem);
                }

                _context.SaveChanges();
                LoadData();
                MessageBox.Show("Акция удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}