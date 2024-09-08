namespace Moshi.MediaWiki.Models;

public class Redirect
{
    public int RedirectId { get; set; }
    public int SourcePageId { get; set; }
    public int TargetPageId { get; set; }
}
