using Microsoft.AspNetCore.Mvc;
using Moshi.PaperTrading.Models;
using Moshi.PaperTrading.Services;

namespace Moshi.PaperTrading.Controllers;
// Controllers/StocksController.cs
[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Stock>>> GetAllStocks()
    {
        return await _stockService.GetAllStocksAsync();
    }

    [HttpGet("{symbol}")]
    public async Task<ActionResult<Stock>> GetStock(string symbol)
    {
        var stock = await _stockService.GetStockAsync(symbol);
        if (stock == null)
            return NotFound();
        return stock;
    }
}