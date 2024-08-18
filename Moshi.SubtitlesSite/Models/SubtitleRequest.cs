namespace Moshi.SubtitlesSite.Models;

public class SubtitleRequest
{
    public int Id { get; set; }
    public string MovieName { get; set; }
    public int Year { get; set; }
    public string LatestSubtitleLanguage { get; set; }
    public float Rating { get; set; }
    public DateTime LatestUploadDate { get; set; }
    public DateTime RequestDate { get; set; }
    public int SubtitleCount { get; set; }
    public int UserId { get; set; }
}