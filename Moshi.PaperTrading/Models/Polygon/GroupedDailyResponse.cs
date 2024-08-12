using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;



public class GroupedDailyResponse
{
    [JsonPropertyName("adjusted")]
    public bool Adjusted { get; set; }

    [JsonPropertyName("queryCount")]
    public int QueryCount { get; set; }

    [JsonPropertyName("results")]
    public OHLC[] Results { get; set; }

    [JsonPropertyName("resultsCount")]
    public int ResultsCount { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}

