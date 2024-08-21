namespace Moshi.MyMusic.Models
{
    public class SearchResult
    {
        public IEnumerable<dynamic> Songs { get; set; }
        public IEnumerable<dynamic> Albums { get; set; }
        public IEnumerable<dynamic> Artists { get; set; }
        public IEnumerable<dynamic> Playlists { get; set; }
        public IEnumerable<dynamic> Podcasts { get; set; }
    }
}