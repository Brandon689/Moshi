using Microsoft.AspNetCore.Mvc;
using Moshi.PaperTrading.Models;
using Moshi.PaperTrading.Services;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly ITradeService _tradeService;

    public TradesController(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<Trade>> ExecuteTrade(string userId, Trade trade)
    {
        var executedTrade = await _tradeService.ExecuteTradeAsync(trade, userId);
        return CreatedAtAction(nameof(GetTradesForUser), new { userId = userId }, executedTrade);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<Trade>>> GetTradesForUser(string userId)
    {
        return await _tradeService.GetTradesForUserAsync(userId);
    }
}