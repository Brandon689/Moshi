namespace Moshi.SubtitlesSite.Models;

public class SubtitleWithMovieDetails
{
    public int SubtitleId { get; set; }
    public string MovieTitle { get; set; }
    public string Username { get; set; }
    public string Language { get; set; }
    public int Downloads { get; set; }
    public decimal ImdbRating { get; set; }
}