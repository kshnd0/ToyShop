using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ToyShop.Models;

namespace ToyShop.Views.UserControls
{
    public partial class EventsView : UserControl
    {
        private ToyShopContext _context;
        private List<EventDisplay> _allEvents;
        private Border _selectedCard;
        private int _selectedEventId = -1;

        public class EventDisplay
        {
            public int IdEvent { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Meaning { get; set; } = "";
        }

        public EventsView()
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
            try
            {
                var events = _context.Events.ToList();
                _allEvents = events.Select(e => new EventDisplay
                {
                    IdEvent = e.IdEvent,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Meaning = e.Meaning ?? ""
                }).ToList();
                icEvents.ItemsSource = _allEvents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                icEvents.ItemsSource = _allEvents;
            }
            else
            {
                var search = tbSearch.Text.ToLower();
                var filtered = _allEvents
                    .Where(ev => ev.Meaning.ToLower().Contains(search))
                    .ToList();
                icEvents.ItemsSource = filtered;
            }
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var card = sender as Border;
                if (card == null) return;

                // Получаем ID из тега
                if (!(card.Tag is int eventId)) return;

                // Снимаем выделение с предыдущей карточки
                if (_selectedCard != null)
                {
                    _selectedCard.BorderBrush = new SolidColorBrush(Color.FromRgb(181, 213, 202));
                    _selectedCard.BorderThickness = new Thickness(1);
                }

                // Выделяем новую карточку
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new EventDialog();
                if (dialog.ShowDialog() == true)
                {
                    var eventItem = new Event
                    {
                        StartDate = dialog.StartDate,
                        EndDate = dialog.EndDate,
                        Meaning = dialog.Meaning
                    };
                    _context.Events.Add(eventItem);
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Акция добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
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

                var dialog = new EventDialog(eventItem);
                if (dialog.ShowDialog() == true)
                {
                    eventItem.StartDate = dialog.StartDate;
                    eventItem.EndDate = dialog.EndDate;
                    eventItem.Meaning = dialog.Meaning;

                    _context.Events.Update(eventItem);
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Акция обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = GetSelectedEvent();
                if (selected == null)
                {
                    MessageBox.Show("Выберите акцию для удаления!", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var eventItem = _context.Events.FirstOrDefault(ev => ev.IdEvent == selected.IdEvent);
                if (eventItem == null) return;

                if (MessageBox.Show($"Удалить акцию \"{selected.Meaning}\"?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Events.Remove(eventItem);
                    _context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Акция удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}