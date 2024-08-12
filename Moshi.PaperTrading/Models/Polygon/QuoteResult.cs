using System.Text.Json.Serialization;

namespace Moshi.PaperTrading.Models.Polygon;

public class QuoteResult
{
    [JsonPropertyName("ask_exchange")]
    public int AskExchange { get; set; }

    [JsonPropertyName("ask_price")]
    public decimal AskPrice { get; set; }

    [JsonPropertyName("ask_size")]
    public decimal AskSize { get; set; }

    [JsonPropertyName("bid_exchange")]
    public int BidExchange { get; set; }

    [JsonPropertyName("bid_price")]
    public decimal BidPrice { get; set; }

    [JsonPropertyName("bid_size")]
    public decimal BidSize { get; set; }

    [JsonPropertyName("conditions")]
    public int[] Conditions { get; set; }

    [JsonPropertyName("indicators")]
    public int[] Indicators { get; set; }

    [JsonPropertyName("participant_timestamp")]
    public long ParticipantTimestamp { get; set; }

    [JsonPropertyName("sequence_number")]
    public long SequenceNumber { get; set; }

    [JsonPropertyName("sip_timestamp")]
    public long SipTimestamp { get; set; }

    [JsonPropertyName("tape")]
    public int Tape { get; set; }

    [JsonPropertyName("trf_timestamp")]
    public long? TrfTimestamp { get; set; }
}