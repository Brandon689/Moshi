namespace Moshi.MediaWiki.Models;

public class PageProtection
{
    public int ProtectionId { get; set; }
    public int PageId { get; set; }
    public string ProtectionType { get; set; }
    public int GroupId { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
