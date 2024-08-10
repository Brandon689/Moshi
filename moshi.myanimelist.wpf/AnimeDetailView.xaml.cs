using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Moshi.MyAnimeList.Models;

namespace AnimeTracker
{
    public partial class AnimeDetailView : UserControl
    {
        private MoshiAnime _currentAnime;
        private UserAnimeStatus? _currentStatus;

        public event EventHandler<MoshiAnime> EditAnimeRequested;
        public event EventHandler<MoshiAnime> AddToMyListRequested;
        public event EventHandler<MoshiAnime> RemoveFromMyListRequested;
        public event EventHandler<(MoshiAnime, UserAnimeStatus)> StatusChangeRequested;

        public AnimeDetailView()
        {
            InitializeComponent();
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(UserAnimeStatus));
        }

        public void UpdateAnimeDetails(UserAnimeListItem animeListItem)
        {
            _currentAnime = animeListItem.Anime;
            _currentStatus = animeListItem.Status;

            AnimeTitle.Text = _currentAnime.Title;
            AnimeType.Text = $"Type: {_currentAnime.Type}";
            AnimeEpisodes.Text = $"Episodes: {_currentAnime.Episodes}";
            AnimeStatus.Text = $"Status: {_currentAnime.Status}";
            AnimeSeason.Text = $"Season: {_currentAnime.AnimeSeason?.Season} {_currentAnime.AnimeSeason?.Year}";

            if (!string.IsNullOrEmpty(_currentAnime.Picture))
            {
                AnimePicture.Source = new BitmapImage(new Uri(_currentAnime.Picture));
            }
            else
            {
                AnimePicture.Source = null;
            }

            TagsItemsControl.ItemsSource = _currentAnime.Tags;

            StatusComboBox.SelectedItem = _currentStatus;
            StatusComboBox.Visibility = _currentStatus.HasValue ? Visibility.Visible : Visibility.Collapsed;
            AddToMyListButton.Visibility = _currentStatus.HasValue ? Visibility.Collapsed : Visibility.Visible;
            RemoveFromMyListButton.Visibility = _currentStatus.HasValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditAnimeRequested?.Invoke(this, _currentAnime);
        }

        private void AddToMyListButton_Click(object sender, RoutedEventArgs e)
        {
            AddToMyListRequested?.Invoke(this, _currentAnime);
            StatusComboBox.Visibility = Visibility.Visible;
            AddToMyListButton.Visibility = Visibility.Collapsed;
            RemoveFromMyListButton.Visibility = Visibility.Visible;
            StatusComboBox.SelectedItem = UserAnimeStatus.PlanToWatch;
        }

        private void RemoveFromMyListButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveFromMyListRequested?.Invoke(this, _currentAnime);
            StatusComboBox.Visibility = Visibility.Collapsed;
            AddToMyListButton.Visibility = Visibility.Visible;
            RemoveFromMyListButton.Visibility = Visibility.Collapsed;
            _currentStatus = null;
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is UserAnimeStatus newStatus && _currentStatus != newStatus)
            {
                _currentStatus = newStatus;
                StatusChangeRequested?.Invoke(this, (_currentAnime, newStatus));
            }
        }
    }
}
