﻿namespace Moshi.PaperTrading.Models.Polygon;

using System.Text.Json.Serialization;

public class AggregateResult
{
    [JsonPropertyName("c")]
    public decimal C { get; set; }

    [JsonPropertyName("h")]
    public decimal H { get; set; }

    [JsonPropertyName("l")]
    public decimal L { get; set; }

    [JsonPropertyName("n")]
    public int N { get; set; }

    [JsonPropertyName("o")]
    public decimal O { get; set; }

    [JsonPropertyName("t")]
    public long T { get; set; }

    [JsonPropertyName("v")]
    public decimal V { get; set; }

    [JsonPropertyName("vw")]
    public decimal Vw { get; set; }
}

