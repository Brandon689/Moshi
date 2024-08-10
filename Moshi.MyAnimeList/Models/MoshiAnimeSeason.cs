using System.Text.Json.Serialization;

namespace Moshi.MyAnimeList.Models;

public class MoshiAnimeSeason
{
    [JsonPropertyName("season")]
    public string Season { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }
}
