namespace Moshi.MyMusic.Models;

public class UserRecommendation
{
    public int UserId { get; set; }
    public string ItemType { get; set; }
    public int ItemId { get; set; }
    public float Score { get; set; }
    public DateTime GeneratedAt { get; set; }
}