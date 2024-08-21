namespace Moshi.MyMusic.Models;

public class UserPreferences
{
    public int UserId { get; set; }
    public string Language { get; set; }
    public string Theme { get; set; }
    public bool NotificationsEnabled { get; set; }
}