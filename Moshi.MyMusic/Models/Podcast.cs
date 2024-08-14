namespace Moshi.MyMusic.Models;

public class Podcast
{
    public int PodcastId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string RssFeedUrl { get; set; }
}