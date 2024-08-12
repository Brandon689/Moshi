namespace Moshi.PaperTrading.Models.Polygon;

using System.Text.Json.Serialization;

public class TradesResponse
{
    [JsonPropertyName("next_url")]
    public string NextUrl { get; set; }

    [JsonPropertyName("results")]
    public TradeResult[] Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}