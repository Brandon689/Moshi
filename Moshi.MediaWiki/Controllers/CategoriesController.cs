using Microsoft.AspNetCore.Mvc;
using Moshi.MediaWiki.Models;
using Moshi.MediaWiki.Services;

namespace Moshi.MediaWiki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetCategory(int id)
    {
        var category = _categoryService.GetCategoryById(id);
        if (category == null)
        {
            return NotFound();
        }
        return category;
    }

    [HttpPost]
    public ActionResult<Category> CreateCategory(Category category)
    {
        try
        {
            _categoryService.CreateCategory(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("page/{pageId}")]
    public ActionResult<IEnumerable<Category>> GetCategoriesForPage(int pageId)
    {
        var categories = _categoryService.GetCategoriesForPage(pageId);
        return Ok(categories);
    }
}