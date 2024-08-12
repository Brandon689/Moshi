using Microsoft.AspNetCore.Mvc;
using Moshi.PaperTrading.Models;
using PaperTradingApp.Services;
using System.Threading.Tasks;

namespace PaperTradingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;

        public PortfoliosController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(string userId)
        {
            var portfolio = await _portfolioService.GetPortfolioAsync(userId);
            if (portfolio == null)
                return NotFound();
            return portfolio;
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<Portfolio>> UpdatePortfolio(string userId, Portfolio portfolio)
        {
            if (userId != portfolio.UserId)
                return BadRequest();

            var updatedPortfolio = await _portfolioService.UpdatePortfolioAsync(portfolio);
            return updatedPortfolio;
        }

        [HttpGet("{userId}/value")]
        public async Task<ActionResult<decimal>> GetPortfolioValue(string userId)
        {
            var value = await _portfolioService.GetPortfolioValueAsync(userId);
            return value;
        }
    }
}