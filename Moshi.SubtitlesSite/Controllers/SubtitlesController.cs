using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;

namespace Moshi.SubtitlesSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubtitlesController : ControllerBase
    {
        private readonly SubtitleService _subtitleService;

        public SubtitlesController(SubtitleService subtitleService)
        {
            _subtitleService = subtitleService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadSubtitle([FromForm] SubtitleUpload model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                var id = await _subtitleService.UploadSubtitle(model);
                return CreatedAtAction(nameof(DownloadSubtitle), new { id }, new { Id = id, FileName = model.File.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading the subtitle: {ex.Message}");
            }
        }

        [HttpGet("{id}/download")]
        public IActionResult DownloadSubtitle(int id)
        {
            try
            {
                var (filePath, contentType, fileName) = _subtitleService.PrepareSubtitleDownload(id);
                return PhysicalFile(filePath, contentType, fileName);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Subtitle file not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while downloading the subtitle: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetSubtitles(int movieId)
        {
            try
            {
                var subtitles = _subtitleService.GetSubtitlesByMovieId(movieId);
                return Ok(subtitles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving subtitles: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSubtitle(int id, [FromBody] SubtitleUpdate model)
        {
            try
            {
                var success = _subtitleService.UpdateSubtitle(id, model);
                if (!success)
                    return StatusCode(500, "Failed to update the subtitle");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the subtitle: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSubtitle(int id)
        {
            try
            {
                var success = _subtitleService.DeleteSubtitle(id);
                if (!success)
                    return StatusCode(500, "Failed to delete the subtitle");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the subtitle: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchSubtitles([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query is required");

            try
            {
                var subtitles = _subtitleService.SearchSubtitles(query);
                return Ok(subtitles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while searching for subtitles: {ex.Message}");
            }
        }

        [HttpPost("{id}/rate")]
        public IActionResult RateSubtitle(int id, [FromBody] SubtitleRatingModel model)
        {
            try
            {
                var success = _subtitleService.RateSubtitle(id, model.UserId, model.Rating);
                if (!success)
                    return StatusCode(500, "Failed to rate the subtitle");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while rating the subtitle: {ex.Message}");
            }
        }

        [HttpGet("top-rated")]
        public IActionResult GetTopRatedSubtitles([FromQuery] int limit = 10)
        {
            try
            {
                var subtitles = _subtitleService.GetTopRatedSubtitles(limit);
                return Ok(subtitles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving top-rated subtitles: {ex.Message}");
            }
        }

        [HttpPost("{id}/comments")]
        public IActionResult AddComment(int id, [FromBody] SubtitleComment model)
        {
            try
            {
                model.SubtitleId = id;
                var success = _subtitleService.AddComment(model);
                if (!success)
                    return StatusCode(500, "Failed to add the comment");
                return CreatedAtAction(nameof(GetComments), new { id }, model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the comment: {ex.Message}");
            }
        }

        [HttpGet("{id}/comments")]
        public IActionResult GetComments(int id)
        {
            try
            {
                var comments = _subtitleService.GetComments(id);
                return Ok(comments);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Subtitle not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving comments: {ex.Message}");
            }
        }
    }

    public class SubtitleRatingModel
    {
        public int UserId { get; set; }
        public int Rating { get; set; }
    }
}