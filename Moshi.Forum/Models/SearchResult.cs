namespace Moshi.Forums.Models;

public class SearchResult
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } // "Thread" or "Post"
    public DateTime CreatedAt { get; set; }
    public string AuthorUsername { get; set; }
    public int ForumId { get; set; }
    public string ForumName { get; set; }
    public int? ThreadId { get; set; } // Null for thread results, populated for post results
}
