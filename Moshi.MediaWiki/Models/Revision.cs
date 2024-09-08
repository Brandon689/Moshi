namespace Moshi.MediaWiki.Models;

public class Revision
{
    public int RevisionId { get; set; }
    public int PageId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public string Comment { get; set; }
    public DateTime Timestamp { get; set; }
}
