using System.Text.Json.Serialization;

namespace Moshi.MyAnimeList.Models;

public class JsonRoot
{
    [JsonPropertyName("data")]
    public List<MoshiAnime> Data { get; set; }
}
