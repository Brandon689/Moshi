using Moshi.PaperTrading.Models.Polygon;
using System.Text.Json;

namespace PolygonTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

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

        //GroupedDailyResponse o = new GroupedDailyResponse();
        //o.Adjusted = true;
        //o.ResultsCount = 10953;
        //o.QueryCount = 10953;
        //o.Results = new OHLC[]
        //{
        //    new OHLC()
        //    {
        //        T = "TTMI",
        //        V= 394280,
        //        Vw= 16.1078m,
        //        O= 15.96m,
        //        C= 16.08m,
        //        H= 16.335m,
        //        L= 15.96m,
        //        t= 1673298000000,
        //        N= 5416
        //    }
        //};
    }
}