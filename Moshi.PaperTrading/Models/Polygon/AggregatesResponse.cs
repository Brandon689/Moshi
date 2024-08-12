namespace Moshi.PaperTrading.Models.Polygon;

using System.Text.Json.Serialization;

public class AggregatesResponse
{
    [JsonPropertyName("adjusted")]
    public bool Adjusted { get; set; }

    [JsonPropertyName("next_url")]
    public string NextUrl { get; set; }

    [JsonPropertyName("queryCount")]
    public int QueryCount { get; set; }

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

    [JsonPropertyName("results")]
    public AggregateResult[] Results { get; set; }

    [JsonPropertyName("resultsCount")]
    public int ResultsCount { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("ticker")]
    public string Ticker { get; set; }
}