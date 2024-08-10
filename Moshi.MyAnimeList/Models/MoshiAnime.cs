using Moshi.MyAnimeList.Models;
using System.Text.Json.Serialization;

public record MoshiAnime
{
    public int AnimeID { get; init; }

    [JsonPropertyName("sources")]
    public List<string> Sources { get; init; } = new List<string>();

    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("episodes")]
    public int Episodes { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; }

    [JsonPropertyName("animeSeason")]
    public MoshiAnimeSeason AnimeSeason { get; init; }

    [JsonPropertyName("picture")]
    public string Picture { get; init; }

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; init; }

    [JsonPropertyName("synonyms")]
    public List<string> Synonyms { get; init; } = new List<string>();

    [JsonPropertyName("relatedAnime")]
    public List<string> RelatedAnime { get; init; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; init; } = new List<string>();

    public MoshiAnime() { }

    public MoshiAnime(int animeID, string title, string type, int episodes, string status,
                      MoshiAnimeSeason animeSeason, string picture, string thumbnail)
    {
        AnimeID = animeID;
        Title = title;
        Type = type;
        Episodes = episodes;
        Status = status;
        AnimeSeason = animeSeason;
        Picture = picture;
        Thumbnail = thumbnail;
    }
}
