using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;

public class LastTradeResponse
{
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

    [JsonPropertyName("results")]
    public LastTradeResult Results { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}