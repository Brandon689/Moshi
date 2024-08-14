namespace Moshi.MyMusic.Models;

public class ListeningHistory
{
    public int HistoryId { get; set; }
    public int UserId { get; set; }
    public int SongId { get; set; }
    public DateTime ListenedAt { get; set; }
    public int DurationListened { get; set; }
}