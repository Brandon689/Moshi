using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AnimeTracker;
using Moshi.MyAnimeList.Models;
namespace AnimeTracker;
public partial class AnimeListView : UserControl
{
    public delegate void AnimeSelectedHandler(UserAnimeListItem selectedAnime);
    public event AnimeSelectedHandler AnimeSelected;

    public AnimeListView()
    {
        InitializeComponent();
    }

    public void UpdateAnimeList(IEnumerable<UserAnimeListItem> animeList)
    {
        AnimeListView1.ItemsSource = new ObservableCollection<UserAnimeListItem>(animeList);
    }

    private void AnimeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AnimeListView1.SelectedItem is UserAnimeListItem selectedAnime)
        {
            AnimeSelected?.Invoke(selectedAnime);
        }
    }
}
