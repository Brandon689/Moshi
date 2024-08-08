namespace Moshi.Forums.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ForumId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Forum Forum { get; set; }
}
