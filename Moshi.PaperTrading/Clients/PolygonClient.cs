namespace Moshi.PaperTrading.Clients;

public class PolygonClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.polygon.io";

    public PolygonClient(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> GetAggregatesAsync(string ticker, int multiplier, string timespan, string from, string to, bool adjusted = true, string sort = "asc", int limit = 5000)
    {
        var url = $"{BaseUrl}/v2/aggs/ticker/{ticker}/range/{multiplier}/{timespan}/{from}/{to}?adjusted={adjusted}&sort={sort}&limit={limit}";
        return await SendRequestAsync(url);
    }

    public async Task<string> GetGroupedDailyAsync(string date, bool adjusted = true, bool includeOtc = false)
    {
        var url = $"{BaseUrl}/v2/aggs/grouped/locale/us/market/stocks/{date}?adjusted={adjusted}&include_otc={includeOtc}";
        return await SendRequestAsync(url);
    }

    public async Task<string> GetDailyOpenCloseAsync(string ticker, string date, bool adjusted = true)
    {
        var url = $"{BaseUrl}/v1/open-close/{ticker}/{date}?adjusted={adjusted}";
        return await SendRequestAsync(url);
    }

    private async Task<string> SendRequestAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}