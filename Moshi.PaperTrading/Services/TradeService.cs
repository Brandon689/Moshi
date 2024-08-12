using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moshi.PaperTrading.Models;
using Moshi.PaperTrading.Services;

namespace PaperTradingApp.Services
{
    public class TradeService : ITradeService
    {
        private readonly ConcurrentDictionary<int, Trade> _trades;
        private readonly IStockService _stockService;
        private readonly IPortfolioService _portfolioService;
        private int _nextTradeId = 1;

        public TradeService(IStockService stockService, IPortfolioService portfolioService)
        {
            _trades = new ConcurrentDictionary<int, Trade>();
            _stockService = stockService;
            _portfolioService = portfolioService;
        }

        public async Task<Trade> ExecuteTradeAsync(Trade trade, string userId)
        {
            // Validate the trade
            var stock = await _stockService.GetStockAsync(trade.StockSymbol);
            if (stock == null)
            {
                throw new ArgumentException("Invalid stock symbol");
            }

            var portfolio = await _portfolioService.GetPortfolioAsync(userId);
            if (portfolio == null)
            {
                throw new ArgumentException("User portfolio not found");
            }

            // Check if user has enough balance for buy or enough stocks for sell
            if (trade.Type == "Buy")
            {
                decimal totalCost = trade.Quantity * stock.CurrentPrice;
                if (portfolio.Balance < totalCost)
                {
                    throw new InvalidOperationException("Insufficient funds");
                }
                portfolio.Balance -= totalCost;
            }
            else if (trade.Type == "Sell")
            {
                int userStockQuantity = _trades.Values
                    .Where(t => t.StockSymbol == trade.StockSymbol)
                    .Sum(t => t.Type == "Buy" ? t.Quantity : -t.Quantity);

                if (userStockQuantity < trade.Quantity)
                {
                    throw new InvalidOperationException("Insufficient stocks");
                }
                portfolio.Balance += trade.Quantity * stock.CurrentPrice;
            }
            else
            {
                throw new ArgumentException("Invalid trade type");
            }

            // Execute the trade
            trade.Id = Interlocked.Increment(ref _nextTradeId);
            trade.Timestamp = DateTime.UtcNow;
            trade.Price = stock.CurrentPrice;

            // Add trade to the collection
            _trades[trade.Id] = trade;

            // Update the user's portfolio
            portfolio.Trades.Add(trade);
            await _portfolioService.UpdatePortfolioAsync(portfolio);

            return trade;
        }

        public async Task<List<Trade>> GetTradesForUserAsync(string userId)
        {
            await Task.CompletedTask; // Simulate async operation

            var portfolio = await _portfolioService.GetPortfolioAsync(userId);
            if (portfolio == null)
            {
                return new List<Trade>();
            }

            return portfolio.Trades.OrderByDescending(t => t.Timestamp).ToList();
        }
    }
}