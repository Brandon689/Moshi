using Moshi.PaperTrading.Models;
using System.Collections.Concurrent;

namespace PaperTradingApp.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly ConcurrentDictionary<string, Portfolio> _portfolios;

        public PortfolioService()
        {
            _portfolios = new ConcurrentDictionary<string, Portfolio>();
        }

        public async Task<Portfolio> GetPortfolioAsync(string userId)
        {
            await Task.CompletedTask; // Simulate async operation

            if (_portfolios.TryGetValue(userId, out var portfolio))
            {
                return portfolio;
            }

            // If the portfolio doesn't exist, create a new one with an initial balance
            var newPortfolio = new Portfolio
            {
                UserId = userId,
                Balance = 100000, // Starting with $100,000 for paper trading
                Trades = new List<Trade>()
            };

            _portfolios[userId] = newPortfolio;
            return newPortfolio;
        }

        public async Task<Portfolio> UpdatePortfolioAsync(Portfolio portfolio)
        {
            await Task.CompletedTask; // Simulate async operation

            if (!_portfolios.TryGetValue(portfolio.UserId, out var existingPortfolio))
            {
                throw new ArgumentException("Portfolio not found for the given user ID");
            }

            // Update the existing portfolio
            existingPortfolio.Balance = portfolio.Balance;
            existingPortfolio.Trades = portfolio.Trades;

            // No need to put it back in the dictionary as it's a reference type

            return existingPortfolio;
        }

        public async Task<decimal> GetPortfolioValueAsync(string userId)
        {
            var portfolio = await GetPortfolioAsync(userId);
            decimal totalValue = portfolio.Balance;

            // Group trades by stock symbol
            var stockHoldings = portfolio.Trades
                .GroupBy(t => t.StockSymbol)
                .Select(g => new
                {
                    Symbol = g.Key,
                    Quantity = g.Sum(t => t.Type == "Buy" ? t.Quantity : -t.Quantity)
                })
                .Where(h => h.Quantity > 0);

            // TODO: Get current stock prices and calculate total value
            // For now, we'll use the last trade price as the current price
            foreach (var holding in stockHoldings)
            {
                var lastTrade = portfolio.Trades
                    .Where(t => t.StockSymbol == holding.Symbol)
                    .OrderByDescending(t => t.Timestamp)
                    .FirstOrDefault();

                if (lastTrade != null)
                {
                    totalValue += holding.Quantity * lastTrade.Price;
                }
            }

            return totalValue;
        }
    }
}