using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;

namespace Moshi.SubtitlesSite.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShowsController : ControllerBase
{
    private readonly ShowsService _showsService;

    public ShowsController(ShowsService showsService)
    {
        _showsService = showsService;
    }

    [HttpGet]
    public IActionResult GetShows()
    {
        var shows = _showsService.GetAllShows();
        return Ok(shows);
    }

    [HttpGet("{id}")]
    public IActionResult GetShow(int id)
    {
        var show = _showsService.GetShowById(id);
        if (show == null)
            return NotFound();
        return Ok(show);
    }

    [HttpPost]
    public IActionResult CreateShow([FromBody] Show show)
    {
        var id = _showsService.CreateShow(show);
        show.Id = id;
        return CreatedAtAction(nameof(GetShow), new { id = show.Id }, show);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateShow(int id, [FromBody] Show show)
    {
        if (id != show.Id)
            return BadRequest("ID mismatch");

        var existingShow = _showsService.GetShowById(id);
        if (existingShow == null)
            return NotFound();

        var success = _showsService.UpdateShow(show);
        if (!success)
            return StatusCode(500, "Failed to update the show");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteShow(int id)
    {
        var existingShow = _showsService.GetShowById(id);
        if (existingShow == null)
            return NotFound();

        var success = _showsService.DeleteShow(id);
        if (!success)
            return StatusCode(500, "Failed to delete the show");

        return NoContent();
    }

    [HttpGet("search")]
    public IActionResult SearchShows([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query is required");

        var shows = _showsService.SearchShows(query);
        return Ok(shows);
    }

    [HttpGet("{id}/subtitles")]
    public IActionResult GetShowSubtitles(int id)
    {
        var show = _showsService.GetShowById(id);
        if (show == null)
            return NotFound("Show not found");

        var subtitles = _showsService.GetShowSubtitles(id);
        return Ok(subtitles);
    }
}