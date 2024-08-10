using System.Collections.ObjectModel;
using Moshi.MyAnimeList.Models;

namespace AnimeTracker
{
    public class UserAnimeListItem
    {
        public MoshiAnime Anime { get; set; }
        public UserAnimeStatus Status { get; set; }
    }

    public class UserAnimeList
    {
        public ObservableCollection<UserAnimeListItem> Anime { get; set; } = new ObservableCollection<UserAnimeListItem>();

        public void AddAnime(MoshiAnime anime, UserAnimeStatus status = UserAnimeStatus.PlanToWatch)
        {
            if (!Anime.Any(item => item.Anime.AnimeID == anime.AnimeID))
            {
                Anime.Add(new UserAnimeListItem { Anime = anime, Status = status });
            }
        }

        public void RemoveAnime(MoshiAnime anime)
        {
            var item = Anime.FirstOrDefault(item => item.Anime.AnimeID == anime.AnimeID);
            if (item != null)
            {
                Anime.Remove(item);
            }
        }

        public void UpdateAnimeStatus(MoshiAnime anime, UserAnimeStatus newStatus)
        {
            var item = Anime.FirstOrDefault(item => item.Anime.AnimeID == anime.AnimeID);
            if (item != null)
            {
                item.Status = newStatus;
            }
        }
    }
}
