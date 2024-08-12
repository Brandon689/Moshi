using JikanDotNet;
using Moshi.MyAnimeList.Models;

namespace Moshi.MyAnimeList.Models
{
    public record JikanAnime(
        int AnimeId,
        long? MalId,
        string Title,
        string TitleEnglish,
        string TitleJapanese,
        string Type,
        int? Episodes,
        string Duration,
        string Rating,
        double? Score,
        int? ScoredBy,
        string Synopsis,
        string Background,
        int? Year
    );
}

public static class DtoMapper
{
    public static JikanAnime ToDto(this Anime entity)
    {
        return new JikanAnime(
            AnimeId: 0,
            MalId: entity.MalId,
            Title: entity.Title,
            TitleEnglish: entity.TitleEnglish,
            TitleJapanese: entity.TitleJapanese,
            Type: entity.Type,
            Episodes: entity.Episodes,
            Duration: entity.Duration,
            Rating: entity.Rating,
            Score: entity.Score,
            ScoredBy: entity.ScoredBy,
            Synopsis: entity.Synopsis,
            Background: entity.Background,
            Year: entity.Year
        );
    }
}