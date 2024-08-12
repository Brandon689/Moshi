using Moshi.PaperTrading.Models;

public interface IPortfolioService
{
    Task<Portfolio> GetPortfolioAsync(string userId);
    Task<Portfolio> UpdatePortfolioAsync(Portfolio portfolio);
    Task<decimal> GetPortfolioValueAsync(string userId);
}