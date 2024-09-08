using Microsoft.AspNetCore.Mvc;
using Moshi.MediaWiki.Models;
using Moshi.MediaWiki.Services;

namespace Moshi.MediaWiki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagesController : ControllerBase
{
    private readonly PageService _pageService;

    public PagesController(PageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet("{id}")]
    public ActionResult<Page> GetPage(int id)
    {
        var page = _pageService.GetPageById(id);
        if (page == null)
        {
            return NotFound();
        }
        return page;
    }

    [HttpPost]
    public ActionResult<Page> CreatePage(Page page)
    {
        try
        {
            _pageService.CreatePage(page);
            return CreatedAtAction(nameof(GetPage), new { id = page.PageId }, page);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}