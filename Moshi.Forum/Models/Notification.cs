namespace Moshi.Forums.Models;

public enum NotificationType
{
    NewPost,
    NewReply,
    Mention,
    ThreadLocked,
    PostDeleted
}

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }

    // Navigation property
    public User User { get; set; }
}
