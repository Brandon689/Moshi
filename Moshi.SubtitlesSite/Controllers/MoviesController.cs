using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;

namespace Moshi.SubtitlesSite.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MoviesService _moviesService;

    public MoviesController(MoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    [HttpGet]
    public IActionResult GetMovies()
    {
        var movies = _moviesService.GetAllMovies();
        return Ok(movies);
    }

    [HttpGet("{id}")]
    public IActionResult GetMovie(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound();
        return Ok(movie);
    }

    [HttpPost]
    public IActionResult CreateMovie([FromBody] Movie movie)
    {
        var id = _moviesService.CreateMovie(movie);
        movie.MovieId = id;
        return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMovie(int id, [FromBody] Movie movie)
    {
        if (id != movie.MovieId)
            return BadRequest("ID mismatch");

        var existingMovie = _moviesService.GetMovieById(id);
        if (existingMovie == null)
            return NotFound();

        var success = _moviesService.UpdateMovie(movie);
        if (!success)
            return StatusCode(500, "Failed to update the movie");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {
        var existingMovie = _moviesService.GetMovieById(id);
        if (existingMovie == null)
            return NotFound();

        var success = _moviesService.DeleteMovie(id);
        if (!success)
            return StatusCode(500, "Failed to delete the movie");

        return NoContent();
    }

    [HttpGet("search")]
    public IActionResult SearchMovies([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query is required");

        var movies = _moviesService.SearchMovies(query);
        return Ok(movies);
    }

    [HttpGet("{id}/subtitles")]
    public IActionResult GetMovieSubtitles(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound("Movie not found");

        var subtitles = _moviesService.GetMovieSubtitles(id);
        return Ok(subtitles);
    }

    [HttpGet("by-letter/{letter}")]
    public IActionResult GetMoviesByLetter(char letter)
    {
        var movies = _moviesService.GetMoviesByFirstLetter(letter);
        return Ok(movies);
    }

    [HttpGet("{id}/alternative-titles")]
    public IActionResult GetAlternativeTitles(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound("Movie not found");

        var alternativeTitles = _moviesService.GetAlternativeTitles(id);
        return Ok(alternativeTitles);
    }

    [HttpPost("{id}/alternative-titles")]
    public IActionResult AddAlternativeTitle(int id, [FromBody] AlternativeTitle alternativeTitle)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound("Movie not found");

        alternativeTitle.MovieId = id;
        var success = _moviesService.AddAlternativeTitle(alternativeTitle);
        if (!success)
            return StatusCode(500, "Failed to add alternative title");

        return CreatedAtAction(nameof(GetAlternativeTitles), new { id = id }, alternativeTitle);
    }

    [HttpGet("{id}/links")]
    public IActionResult GetMovieLinks(int id)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound("Movie not found");

        var links = _moviesService.GetMovieLinks(id);
        return Ok(links);
    }

    [HttpPost("{id}/links")]
    public IActionResult AddMovieLink(int id, [FromBody] MovieLink movieLink)
    {
        var movie = _moviesService.GetMovieById(id);
        if (movie == null)
            return NotFound("Movie not found");

        movieLink.MovieId = id;
        var success = _moviesService.AddMovieLink(movieLink);
        if (!success)
            return StatusCode(500, "Failed to add movie link");

        return CreatedAtAction(nameof(GetMovieLinks), new { id = id }, movieLink);
    }
}