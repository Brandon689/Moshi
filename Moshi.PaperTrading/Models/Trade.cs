namespace Moshi.PaperTrading.Models;

public class Trade
{
    public int Id { get; set; }
    public string StockSymbol { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } // "Buy" or "Sell"
}