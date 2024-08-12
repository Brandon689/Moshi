using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;

public class QuotesResponse
{
    [JsonPropertyName("next_url")]
    public string NextUrl { get; set; }

    [JsonPropertyName("results")]
    public QuoteResult[] Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}