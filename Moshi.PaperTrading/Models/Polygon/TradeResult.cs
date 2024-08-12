namespace Moshi.PaperTrading.Models.Polygon;

using System.Text.Json.Serialization;

public class TradeResult
{
    [JsonPropertyName("conditions")]
    public int[] Conditions { get; set; }

    [JsonPropertyName("correction")]
    public int? Correction { get; set; }

    [JsonPropertyName("exchange")]
    public int Exchange { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("participant_timestamp")]
    public long ParticipantTimestamp { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("sequence_number")]
    public long SequenceNumber { get; set; }

    [JsonPropertyName("sip_timestamp")]
    public long SipTimestamp { get; set; }

    [JsonPropertyName("size")]
    public decimal Size { get; set; }

    [JsonPropertyName("tape")]
    public int? Tape { get; set; }

    [JsonPropertyName("trf_id")]
    public int? TrfId { get; set; }

    [JsonPropertyName("trf_timestamp")]
    public long? TrfTimestamp { get; set; }
}
