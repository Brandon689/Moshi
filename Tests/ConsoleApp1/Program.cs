using Moshi.PaperTrading.Models.Polygon;
using System.Text.Json;

string json = @"{
    ""ticker"": ""FB"",
    ""queryCount"": 1,
    ""resultsCount"": 1,
    ""adjusted"": true,
    ""results"":
    [
        {
            ""T"": ""FB"",
            ""v"": 22267154,
            ""vw"": 198.3613,
            ""o"": 194.67,
            ""c"": 196.64,
            ""h"": 202.03,
            ""l"": 194.41,
            ""t"": 1654718400000,
            ""n"": 247919
        }
    ],
    ""status"": ""OK"",
    ""request_id"": ""570d18ccc9ac1a5af52824a70f1108a7"",
    ""count"": 1
}";

var a = JsonSerializer.Deserialize<PreviousCloseResponse>(json);
;