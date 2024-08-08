namespace Moshi.Forums.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsBanned { get; set; }
    public DateTime? BanExpiresAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> Roles { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<ForumThread> Threads { get; set; }
    public ICollection<Post> Posts { get; set; }
}
