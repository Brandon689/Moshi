using Moshi.PaperTrading.Models;

namespace Moshi.PaperTrading.Services;

// Services/IStockService.cs
public interface IStockService
{
    Task<Stock> GetStockAsync(string symbol);
    Task<List<Stock>> GetAllStocksAsync();
}
