namespace Moshi.Forums.Models;
public class ForumThread
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int ForumId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public int ReplyCount { get; set; }

    // Navigation properties
    // public virtual Forum Forum { get; set; }
    // public virtual User User { get; set; }
    // public virtual ICollection<Post> Posts { get; set; }
}
