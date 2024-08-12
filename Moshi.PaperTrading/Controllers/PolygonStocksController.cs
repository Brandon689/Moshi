using Microsoft.AspNetCore.Mvc;
using Moshi.PaperTrading.Models.Polygon;
using Moshi.PaperTrading.Services;

namespace Moshi.PaperTrading.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolygonStocksController : ControllerBase
{
    private readonly PolygonService _polygonService;

    public PolygonStocksController(PolygonService polygonService)
    {
        _polygonService = polygonService;
    }

    [HttpGet("aggregates/{ticker}")]
    public async Task<ActionResult<AggregatesResponse>> GetAggregates(
        string ticker,
        [FromQuery] int multiplier,
        [FromQuery] string timespan,
        [FromQuery] string from,
        [FromQuery] string to)
    {
        try
        {
            var result = await _polygonService.GetAggregatesAsync(ticker, multiplier, timespan, from, to);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("grouped-daily/{date}")]
    public async Task<ActionResult<GroupedDailyResponse>> GetGroupedDaily(string date)
    {
        try
        {
            var result = await _polygonService.GetGroupedDailyAsync(date);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("daily-open-close/{ticker}/{date}")]
    public async Task<ActionResult<DailyOpenCloseResponse>> GetDailyOpenClose(string ticker, string date)
    {
        try
        {
            var result = await _polygonService.GetDailyOpenCloseAsync(ticker, date);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("previous-close/{ticker}")]
    public async Task<ActionResult<PreviousCloseResponse>> GetPreviousClose(string ticker, [FromQuery] bool adjusted = true)
    {
        try
        {
            var result = await _polygonService.GetPreviousCloseAsync(ticker, adjusted);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("trades/{ticker}")]
    public async Task<ActionResult<TradesResponse>> GetTrades(string ticker, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _polygonService.GetTradesAsync(ticker, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("last-trade/{ticker}")]
    public async Task<ActionResult<LastTradeResponse>> GetLastTrade(string ticker)
    {
        try
        {
            var result = await _polygonService.GetLastTradeAsync(ticker);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("quotes/{ticker}")]
    public async Task<ActionResult<QuotesResponse>> GetQuotes(string ticker, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _polygonService.GetQuotesAsync(ticker, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("last-quote/{ticker}")]
    public async Task<ActionResult<LastQuoteResponse>> GetLastQuote(string ticker)
    {
        try
        {
            var result = await _polygonService.GetLastQuoteAsync(ticker);
            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}