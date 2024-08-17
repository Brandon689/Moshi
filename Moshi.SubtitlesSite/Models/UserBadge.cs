namespace Moshi.SubtitlesSite.Models;

public class UserBadge
{
    public int BadgeId { get; set; }
    public int UserId { get; set; }
    public string BadgeName { get; set; }
    public DateTime AwardDate { get; set; }
}