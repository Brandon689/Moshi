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
}