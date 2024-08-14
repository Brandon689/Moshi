namespace Moshi.MyMusic.Models;

public class Album
{
    public int AlbumId { get; set; }
    public string Title { get; set; }
    public int ArtistId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public string Label { get; set; }
}