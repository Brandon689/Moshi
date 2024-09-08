namespace Moshi.MediaWiki.Models;

public class Link
{
    public int LinkId { get; set; }
    public int SourcePageId { get; set; }
    public int TargetPageId { get; set; }
    public string LinkText { get; set; }
}
