namespace Moshi.Forums.Models;

public class PostSearchResult
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public int ThreadId { get; set; }
    public string ThreadTitle { get; set; }
    public int ForumId { get; set; }
    public string ForumName { get; set; }
}