using JikanDotNet;

namespace Moshi.MyAnimeList.JikanClients
{
    public class JikanClient
    {
        private readonly Jikan _jikan = new();

        public async Task<Anime> GetAnime(int id)
        {
            var anime = await _jikan.GetAnimeAsync(id);
            return anime.Data;
        }
    }
}