namespace SubtitlesSiteMVCFrontend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;
using System;
using System.Threading.Tasks;

public class SubtitlesController : Controller
{
    private readonly SubtitleService _subtitleService;
    private readonly SubtitleParserService _subtitleParserService;
    private readonly MoviesService _moviesService;

    public SubtitlesController(SubtitleService subtitleService, SubtitleParserService subtitleParserService, MoviesService moviesService)
    {
        _subtitleService = subtitleService;
        _subtitleParserService = subtitleParserService;
        _moviesService = moviesService;
    }

    // GET: Subtitles/Upload
    public IActionResult Upload()
    {
        ViewBag.Movies = _moviesService.GetAllMovies();
        return View();
    }

    // POST: Subtitles/Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(SubtitleUpload model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var id = await _subtitleService.UploadSubtitle(model);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error uploading subtitle: " + ex.Message);
            }
        }
        ViewBag.Movies = _moviesService.GetAllMovies();
        return View(model);
    }

    // GET: Subtitles/Details/5
    public IActionResult Details(int id)
    {
        var subtitle = _subtitleService.GetSubtitleById(id);
        if (subtitle == null)
        {
            return NotFound();
        }
        return View(subtitle);
    }

    // GET: Subtitles/Edit/5
    public IActionResult Edit(int id)
    {
        var subtitle = _subtitleService.GetSubtitleById(id);
        if (subtitle == null)
        {
            return NotFound();
        }
        return View(subtitle);
    }

    // POST: Subtitles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, SubtitleUpdate model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var success = _subtitleService.UpdateSubtitle(id, model);
                if (success)
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating subtitle: " + ex.Message);
            }
        }
        return View(model);
    }

    // POST: Subtitles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            var success = _subtitleService.DeleteSubtitle(id);
            if (success)
            {
                return RedirectToAction("Index", "Movies");
            }
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error deleting subtitle: " + ex.Message);
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: Subtitles/Download/5
    public IActionResult Download(int id)
    {
        try
        {
            var (filePath, contentType, fileName) = _subtitleService.PrepareSubtitleDownload(id);
            return PhysicalFile(filePath, contentType, fileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FileNotFoundException)
        {
            return NotFound("Subtitle file not found.");
        }
    }

    // POST: Subtitles/Rate/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Rate(int id, SubtitleRating model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var success = _subtitleService.RateSubtitle(id, model.UserId, model.Rating);
                if (success)
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: Subtitles/Search
    public IActionResult Search(string query, char? letter)
    {
        IEnumerable<Movie> movies;
        if (!string.IsNullOrEmpty(query))
        {
            movies = _moviesService.SearchMovies(query);
        }
        else if (letter.HasValue)
        {
            movies = _moviesService.GetMoviesByFirstLetter(letter.Value);
        }
        else
        {
            movies = _moviesService.GetAllMovies();
        }

        ViewBag.CurrentLetter = letter;
        return View("Index", movies);
    }

    // GET: Subtitles/TopRated
    public IActionResult TopRated(int limit = 10)
    {
        var subtitles = _subtitleService.GetTopRatedSubtitles(limit);
        return View(subtitles);
    }

    // GET: Subtitles/Parse/5
    public IActionResult Parse(int id)
    {
        try
        {
            var subtitle = _subtitleService.GetSubtitleById(id);
            if (subtitle == null)
            {
                return NotFound("Subtitle not found.");
            }

            var (filePath, _, _) = _subtitleService.PrepareSubtitleDownload(id);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Subtitle file not found.");
            }

            var jsonContent = _subtitleParserService.ParseSubtitleToJson(filePath);

            return Content(jsonContent, "application/json");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, "An error occurred while parsing the subtitle.");
        }
    }

    // POST: Subtitles/AddComment/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int id, SubtitleComment model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                model.SubtitleId = id;
                var success = _subtitleService.AddComment(model);
                if (success)
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding comment: " + ex.Message);
            }
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: Subtitles/GetComments/5
    public IActionResult GetComments(int id)
    {
        try
        {
            var comments = _subtitleService.GetComments(id);
            return PartialView("_Comments", comments);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving comments.");
        }
    }
}