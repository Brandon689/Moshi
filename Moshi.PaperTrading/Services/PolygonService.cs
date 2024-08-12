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

    public async Task<PreviousCloseResponse> GetPreviousCloseAsync(string ticker, bool adjusted = true)
    {
        var json = await _client.GetPreviousCloseAsync(ticker, adjusted);
        return JsonSerializer.Deserialize<PreviousCloseResponse>(json);
    }

    public async Task<TradesResponse> GetTradesAsync(string ticker, int limit = 10)
    {
        var json = await _client.GetTradesAsync(ticker, limit);
        return JsonSerializer.Deserialize<TradesResponse>(json);
    }

    public async Task<LastTradeResponse> GetLastTradeAsync(string ticker)
    {
        var json = await _client.GetLastTradeAsync(ticker);
        return JsonSerializer.Deserialize<LastTradeResponse>(json);
    }

    public async Task<QuotesResponse> GetQuotesAsync(string ticker, int limit = 10)
    {
        var json = await _client.GetQuotesAsync(ticker, limit);
        return JsonSerializer.Deserialize<QuotesResponse>(json);
    }

    public async Task<LastQuoteResponse> GetLastQuoteAsync(string ticker)
    {
        var json = await _client.GetLastQuoteAsync(ticker);
        return JsonSerializer.Deserialize<LastQuoteResponse>(json);
    }
}

public class PolygonOptions
{
    public string ApiKey { get; set; }
}