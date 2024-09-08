using Microsoft.AspNetCore.Mvc;
using Moshi.LEGO_Catalog.Services;
using System.Threading.Tasks;

namespace Moshi.LEGO_Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        // Categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var categories = await _catalogService.GetAllCategoriesAsync(page, pageSize);
            return Ok(categories);
        }

        [HttpGet("categories/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            var category = await _catalogService.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // Colors
        [HttpGet("colors")]
        public async Task<IActionResult> GetAllColors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var colors = await _catalogService.GetAllColorsAsync(page, pageSize);
            return Ok(colors);
        }

        [HttpGet("colors/{colorId}")]
        public async Task<IActionResult> GetColorById(string colorId)
        {
            var color = await _catalogService.GetColorByIdAsync(colorId);
            if (color == null)
                return NotFound();
            return Ok(color);
        }

        // ItemTypes
        [HttpGet("itemtypes")]
        public async Task<IActionResult> GetAllItemTypes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var itemTypes = await _catalogService.GetAllItemTypesAsync(page, pageSize);
            return Ok(itemTypes);
        }

        // Parts
        [HttpGet("parts")]
        public async Task<IActionResult> GetAllParts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var parts = await _catalogService.GetAllPartsAsync(page, pageSize);
            return Ok(parts);
        }

        [HttpGet("parts/{partNumber}")]
        public async Task<IActionResult> GetPartByNumber(string partNumber)
        {
            var part = await _catalogService.GetPartByNumberAsync(partNumber);
            if (part == null)
                return NotFound();
            return Ok(part);
        }

        // Sets
        [HttpGet("sets")]
        public async Task<IActionResult> GetAllSets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var sets = await _catalogService.GetAllSetsAsync(page, pageSize);
            return Ok(sets);
        }

        [HttpGet("sets/{setNumber}")]
        public async Task<IActionResult> GetSetByNumber(string setNumber)
        {
            var set = await _catalogService.GetSetByNumberAsync(setNumber);
            if (set == null)
                return NotFound();
            return Ok(set);
        }

        // Minifigures
        [HttpGet("minifigures")]
        public async Task<IActionResult> GetAllMinifigures([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var minifigures = await _catalogService.GetAllMinifiguresAsync(page, pageSize);
            return Ok(minifigures);
        }

        [HttpGet("minifigures/{minifigureNumber}")]
        public async Task<IActionResult> GetMinifigureByNumber(string minifigureNumber)
        {
            var minifigure = await _catalogService.GetMinifigureByNumberAsync(minifigureNumber);
            if (minifigure == null)
                return NotFound();
            return Ok(minifigure);
        }

        // Codes
        [HttpGet("codes")]
        public async Task<IActionResult> GetAllCodes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var codes = await _catalogService.GetAllCodesAsync(page, pageSize);
            return Ok(codes);
        }

        [HttpGet("codes/{itemNo}/{color}")]
        public async Task<IActionResult> GetCodeByItemNoAndColor(string itemNo, string color)
        {
            var code = await _catalogService.GetCodeByItemNoAndColorAsync(itemNo, color);
            if (code == null)
                return NotFound();
            return Ok(code);
        }
    }
}