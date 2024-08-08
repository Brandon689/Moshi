namespace Moshi.Forums.Models.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int PostCount { get; set; }
    public int ThreadCount { get; set; }
}
