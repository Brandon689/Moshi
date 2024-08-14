namespace Moshi.MyMusic.Models;

public class UserLibraryItem
{
    public int UserId { get; set; }
    public string ItemType { get; set; }
    public int ItemId { get; set; }
    public DateTime AddedAt { get; set; }
}