namespace Moshi.MyMusic.Models;

public class UserFollower
{
    public int FollowerId { get; set; }
    public int FollowedId { get; set; }
    public DateTime FollowedAt { get; set; }
}