namespace Moshi.MyMusic.Models;

public class Playlist
{
    public int PlaylistId { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}