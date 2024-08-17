namespace Moshi.SubtitlesSite.Models;

public class MovieLink
{
    public int LinkId { get; set; }
    public int MovieId { get; set; }
    public string LinkType { get; set; }
    public string Url { get; set; }
}