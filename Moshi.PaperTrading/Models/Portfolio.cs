namespace Moshi.PaperTrading.Models;

public class Portfolio
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public decimal Balance { get; set; }
    public List<Trade> Trades { get; set; }
}
