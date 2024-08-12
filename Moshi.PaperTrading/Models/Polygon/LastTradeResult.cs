using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;

public class LastTradeResult
{
    [JsonPropertyName("T")]
    public string Ticker { get; set; }

    [JsonPropertyName("c")]
    public int[] Conditions { get; set; }

    [JsonPropertyName("e")]
    public int TradeCorrection { get; set; }

    [JsonPropertyName("f")]
    public long TrfTimestamp { get; set; }

    [JsonPropertyName("i")]
    public string TradeId { get; set; }

    [JsonPropertyName("p")]
    public decimal Price { get; set; }

    [JsonPropertyName("q")]
    public long SequenceNumber { get; set; }

    [JsonPropertyName("r")]
    public int TrfId { get; set; }

    [JsonPropertyName("s")]
    public decimal Size { get; set; }

    [JsonPropertyName("t")]
    public long SipTimestamp { get; set; }

    [JsonPropertyName("x")]
    public int ExchangeId { get; set; }

    [JsonPropertyName("y")]
    public long ParticipantTimestamp { get; set; }

    [JsonPropertyName("z")]
    public int Tape { get; set; }
}