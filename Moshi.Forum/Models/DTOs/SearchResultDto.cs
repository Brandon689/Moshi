namespace Moshi.Forums.Models.DTOs;

public class SearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Type { get; set; } // "Thread" or "Post"
    public DateTime CreatedAt { get; set; }
    public string AuthorUsername { get; set; }
}
