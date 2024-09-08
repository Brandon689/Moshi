namespace Moshi.MediaWiki.Models;

public class Page
{
    public int PageId { get; set; }
    public string Title { get; set; }
    public int? CurrentRevisionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
}
