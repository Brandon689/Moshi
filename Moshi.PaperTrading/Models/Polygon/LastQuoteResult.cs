using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;

public class LastQuoteResult
{
    [JsonPropertyName("P")]
    public decimal AskPrice { get; set; }

    [JsonPropertyName("S")]
    public int AskSize { get; set; }

    [JsonPropertyName("T")]
    public string Ticker { get; set; }

    [JsonPropertyName("X")]
    public int AskExchange { get; set; }

    [JsonPropertyName("c")]
    public int[] Conditions { get; set; }

    [JsonPropertyName("f")]
    public long TrfTimestamp { get; set; }

    [JsonPropertyName("i")]
    public int[] Indicators { get; set; }

    [JsonPropertyName("p")]
    public decimal BidPrice { get; set; }

    [JsonPropertyName("q")]
    public long SequenceNumber { get; set; }

    [JsonPropertyName("s")]
    public int BidSize { get; set; }

    [JsonPropertyName("t")]
    public long SipTimestamp { get; set; }

    [JsonPropertyName("x")]
    public int BidExchange { get; set; }

    [JsonPropertyName("y")]
    public long ParticipantTimestamp { get; set; }

    [JsonPropertyName("z")]
    public int Tape { get; set; }
}
