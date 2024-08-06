namespace Moshi.Forums.Models;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    // public virtual ICollection<Thread> Threads { get; set; }
    // public virtual ICollection<Post> Posts { get; set; }
}
