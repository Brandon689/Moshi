using System;
using System.Windows;
using Moshi.MyAnimeList.Models;

namespace AnimeTracker
{
    public partial class MainWindow : Window
    {
        private readonly ApiClient _apiClient;
        private readonly AnimeListView _myListView;
        private readonly AnimeListView _allAnimeView;
        private readonly UserAnimeList _userAnimeList;

        public MainWindow()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _userAnimeList = new UserAnimeList();
            _myListView = new AnimeListView();
            _allAnimeView = new AnimeListView();

            _myListView.AnimeSelected += OnAnimeSelected;
            _allAnimeView.AnimeSelected += OnAnimeSelected;

            MyListContent.Content = _myListView;
            AllAnimeContent.Content = _allAnimeView;

            LoadAllAnime();
            UpdateMyList();
        }

        private async void LoadAllAnime()
        {
            try
            {
                var allAnime = await _apiClient.GetAllAnimeAsync();
                _allAnimeView.UpdateAnimeList(allAnime.Select(a => new UserAnimeListItem { Anime = a, Status = UserAnimeStatus.PlanToWatch }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading anime: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateMyList()
        {
            _myListView.UpdateAnimeList(_userAnimeList.Anime);
        }

        private void OnAnimeSelected(UserAnimeListItem selectedAnime)
        {
            var detailView = new AnimeDetailView();
            detailView.UpdateAnimeDetails(selectedAnime);
            detailView.EditAnimeRequested += OnEditAnimeRequested;
            detailView.AddToMyListRequested += OnAddToMyListRequested;
            detailView.RemoveFromMyListRequested += OnRemoveFromMyListRequested;
            detailView.StatusChangeRequested += OnStatusChangeRequested;

            var detailWindow = new Window
            {
                Title = "Anime Details",
                Content = detailView,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = this
            };
            detailWindow.ShowDialog();
        }

        private void OnEditAnimeRequested(object sender, MoshiAnime anime)
        {
            var editWindow = new AddEditAnimeWindow(_apiClient, anime);
            if (editWindow.ShowDialog() == true)
            {
                LoadAllAnime();
                UpdateMyList();
            }
        }

        private void OnAddToMyListRequested(object sender, MoshiAnime anime)
        {
            _userAnimeList.AddAnime(anime);
            UpdateMyList();
        }

        private void OnRemoveFromMyListRequested(object sender, MoshiAnime anime)
        {
            _userAnimeList.RemoveAnime(anime);
            UpdateMyList();
        }

        private void OnStatusChangeRequested(object sender, (MoshiAnime anime, UserAnimeStatus status) update)
        {
            _userAnimeList.UpdateAnimeStatus(update.anime, update.status);
            UpdateMyList();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchResults = await _apiClient.SearchAnimeAsync(SearchBox.Text);
                _allAnimeView.UpdateAnimeList(searchResults.Select(a => new UserAnimeListItem { Anime = a, Status = UserAnimeStatus.PlanToWatch }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching anime: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditAnimeWindow(_apiClient);
            if (addEditWindow.ShowDialog() == true)
            {
                LoadAllAnime();
            }
        }
    }
}

