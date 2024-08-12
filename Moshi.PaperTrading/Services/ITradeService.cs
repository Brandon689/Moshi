using Moshi.PaperTrading.Models;

namespace Moshi.PaperTrading.Services;

// Services/ITradeService.cs
public interface ITradeService
{
    Task<Trade> ExecuteTradeAsync(Trade trade, string userId);
    Task<List<Trade>> GetTradesForUserAsync(string userId);
}
