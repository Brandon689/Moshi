namespace Moshi.MyMusic.Models;

public class Song
{
    public int SongId { get; set; }
    public string Title { get; set; }
    public int? AlbumId { get; set; }
    public int Duration { get; set; }
    public int? TrackNumber { get; set; }
    public bool Explicit { get; set; }
    public string Lyrics { get; set; }
    public string AudioFilePath { get; set; }
}