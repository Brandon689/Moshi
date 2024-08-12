using Microsoft.Extensions.Options;
using Moshi.PaperTrading.Clients;
using Moshi.PaperTrading.Models.Polygon;
using System.Text.Json;

namespace Moshi.PaperTrading.Services;

public class PolygonService
{
    private readonly PolygonClient _client;

    public PolygonService(IOptions<PolygonOptions> options)
    {
        _client = new PolygonClient(options.Value.ApiKey);
    }

    public async Task<AggregatesResponse> GetAggregatesAsync(string ticker, int multiplier, string timespan, string from, string to)
    {
        var json = await _client.GetAggregatesAsync(ticker, multiplier, timespan, from, to);
        Console.WriteLine(json);
        return JsonSerializer.Deserialize<AggregatesResponse>(json);
    }

    public async Task<GroupedDailyResponse> GetGroupedDailyAsync(string date)
    {
        var json = await _client.GetGroupedDailyAsync(date);
        return JsonSerializer.Deserialize<GroupedDailyResponse>(json);
    }

    public async Task<DailyOpenCloseResponse> GetDailyOpenCloseAsync(string ticker, string date)
    {
        var json = await _client.GetDailyOpenCloseAsync(ticker, date);
        return JsonSerializer.Deserialize<DailyOpenCloseResponse>(json);
    }
}

public class PolygonOptions
{
    public string ApiKey { get; set; }
}