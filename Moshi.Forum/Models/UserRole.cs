namespace Moshi.Forums.Models;

public enum Role
{
    User,
    Moderator,
    Admin
}

public class UserRole
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Role Role { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation property
    public User User { get; set; }
}
