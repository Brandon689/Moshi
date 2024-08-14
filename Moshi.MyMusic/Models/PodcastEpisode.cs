namespace Moshi.MyMusic.Models;

public class PodcastEpisode
{
    public int EpisodeId { get; set; }
    public int PodcastId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string AudioFilePath { get; set; }
}