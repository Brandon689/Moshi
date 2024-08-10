using System.Text.Json.Serialization;

namespace Moshi.MyAnimeList.Models;

public class MoshiAnime
{
    public int AnimeID { get; set; }
    [JsonPropertyName("sources")]
    public List<string> Sources { get; set; } = new List<string>();

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("episodes")]
    public int Episodes { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("animeSeason")]
    public MoshiAnimeSeason AnimeSeason { get; set; }

    [JsonPropertyName("picture")]
    public string Picture { get; set; }

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }

    [JsonPropertyName("synonyms")]
    public List<string> Synonyms { get; set; } = new List<string>();

    [JsonPropertyName("relatedAnime")]
    public List<string> RelatedAnime { get; set; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}