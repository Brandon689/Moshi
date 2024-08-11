using Moshi.MyAnimeList.Models;
using System.Windows;

namespace AnimeTracker
{
    public partial class AddEditAnimeWindow : Window
    {
        private readonly ApiClient _apiClient;
        private MoshiAnime _animeToEdit;

        public AddEditAnimeWindow(ApiClient apiClient, MoshiAnime animeToEdit = null)
        {
            InitializeComponent();
            _apiClient = apiClient;
            _animeToEdit = animeToEdit;

            InitializeComboBoxes();

            if (animeToEdit != null)
            {
                PopulateFields(animeToEdit);
            }
        }

        private void InitializeComboBoxes()
        {
            TypeComboBox.ItemsSource = new[] { "TV", "Movie", "OVA", "Special", "ONA" };
            StatusComboBox.ItemsSource = new[] { "Finished", "Currently Airing", "Not Yet Aired" };
            SeasonComboBox.ItemsSource = new[] { "Winter", "Spring", "Summer", "Fall" };
        }

        private void PopulateFields(MoshiAnime anime)
        {
            TitleTextBox.Text = anime.Title;
            TypeComboBox.SelectedItem = anime.Type;
            EpisodesTextBox.Text = anime.Episodes.ToString();
            StatusComboBox.SelectedItem = anime.Status;
            SeasonComboBox.SelectedItem = anime.AnimeSeason?.Season;
            YearTextBox.Text = anime.AnimeSeason?.Year.ToString();
            PictureUrlTextBox.Text = anime.Picture;
            TagsTextBox.Text = string.Join(Environment.NewLine, anime.Tags);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var anime = new MoshiAnime
                {
                    Title = TitleTextBox.Text,
                    Type = TypeComboBox.SelectedItem as string,
                    Episodes = int.Parse(EpisodesTextBox.Text),
                    Status = StatusComboBox.SelectedItem as string,
                    AnimeSeason = new MoshiAnimeSeason
                    {
                        Season = SeasonComboBox.SelectedItem as string,
                        Year = int.Parse(YearTextBox.Text)
                    },
                    Picture = PictureUrlTextBox.Text,
                    Tags = TagsTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList()
                };

                if (_animeToEdit == null)
                {
                    await _apiClient.AddAnimeAsync(anime);
                }
                else
                {
                    //await _apiClient.UpdateAnimeAsync(_animeToEdit.Id, anime);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving anime: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
