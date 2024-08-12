using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moshi.PaperTrading.Models;
using Moshi.PaperTrading.Services;

namespace PaperTradingApp.Services
{
    public class StockService : IStockService
    {
        private readonly ConcurrentDictionary<string, Stock> _stocks;

        public StockService()
        {
            // Initialize with some mock data
            _stocks = new ConcurrentDictionary<string, Stock>(
                new Dictionary<string, Stock>
                {
                    {"AAPL", new Stock { Symbol = "AAPL", Name = "Apple Inc.", CurrentPrice = 150.00m }},
                    {"GOOGL", new Stock { Symbol = "GOOGL", Name = "Alphabet Inc.", CurrentPrice = 2800.00m }},
                    {"MSFT", new Stock { Symbol = "MSFT", Name = "Microsoft Corporation", CurrentPrice = 300.00m }},
                    {"AMZN", new Stock { Symbol = "AMZN", Name = "Amazon.com, Inc.", CurrentPrice = 3300.00m }},
                    {"FB", new Stock { Symbol = "FB", Name = "Meta Platforms, Inc.", CurrentPrice = 330.00m }}
                });
        }

        public async Task<Stock> GetStockAsync(string symbol)
        {
            // Simulate async operation
            await Task.Delay(10);

            _stocks.TryGetValue(symbol.ToUpper(), out var stock);
            return stock;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            // Simulate async operation
            await Task.Delay(10);

            return _stocks.Values.ToList();
        }

        // You might want to add a method to update stock prices periodically
        // This is just a simple implementation for demonstration
        public async Task UpdateStockPriceAsync(string symbol, decimal newPrice)
        {
            await Task.Delay(10);

            if (_stocks.TryGetValue(symbol.ToUpper(), out var stock))
            {
                stock.CurrentPrice = newPrice;
                _stocks[symbol.ToUpper()] = stock;
            }
        }
    }
}