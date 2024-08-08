namespace Moshi.Forums.Models;

public class ThreadSearchResult
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; }
    public int ForumId { get; set; }
    public string ForumName { get; set; }
}