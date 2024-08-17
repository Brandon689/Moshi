namespace Moshi.SubtitlesSite.Models;

public class Movie
{
    public int MovieId { get; set; }
    public string ImdbId { get; set; }
    public string Title { get; set; }
    public string OriginalTitle { get; set; }
    public int Year { get; set; }
    public string Synopsis { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Writers { get; set; }
    public string Cast { get; set; }
    public int? Duration { get; set; }
    public string Language { get; set; }
    public string Country { get; set; }
    public double? ImdbRating { get; set; }
    public string PosterUrl { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? LastUpdated { get; set; }
    public int SubtitleCount { get; set; }
}