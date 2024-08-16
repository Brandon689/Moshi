namespace Moshi.SubtitlesSite.Models;

public class Show
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Type { get; set; } // e.g., "Movie", "TV Series", "Anime", etc.
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Cast { get; set; }
    public int? NumberOfSeasons { get; set; } // Nullable for movies
    public int? NumberOfEpisodes { get; set; } // Nullable for movies
    public string Language { get; set; }
    public string Country { get; set; }
    public decimal? Rating { get; set; } // e.g., IMDb rating
    public string PosterUrl { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? LastUpdated { get; set; }
}