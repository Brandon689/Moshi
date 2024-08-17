namespace Moshi.SubtitlesSite.Models;

public class SubtitleRating
{
    public int RatingId { get; set; }
    public int SubtitleId { get; set; }
    public int UserId { get; set; }
    public float Rating { get; set; }
    public DateTime RatingDate { get; set; }
}