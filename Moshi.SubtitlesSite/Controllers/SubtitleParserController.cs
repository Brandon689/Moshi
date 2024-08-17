using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Services;

namespace Moshi.SubtitlesSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubtitleParserController : ControllerBase
    {
        private readonly SubtitleParserService _parserService;
        private readonly SubtitleRepository _repository;
        private readonly string _uploadPath;

        public SubtitleParserController(SubtitleParserService parserService, SubtitleRepository repository, IWebHostEnvironment env)
        {
            _parserService = parserService;
            _repository = repository;
            _uploadPath = Path.Combine(env.ContentRootPath, "Uploads");
        }

        [HttpGet("{id}/parsed")]
        public IActionResult GetParsedSubtitle(int id)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                return NotFound("Subtitle not found.");

            var filePath = Path.Combine(_uploadPath, subtitle.StorageFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Subtitle file not found.");

            try
            {
                var parsedContent = _parserService.ParseSubtitleToJson(filePath);
                return Content(parsedContent, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error parsing subtitle: {ex.Message}");
            }
        }

        [HttpGet("{id}/plaintext")]
        public IActionResult GetPlainTextSubtitle(int id)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                return NotFound("Subtitle not found.");

            var filePath = Path.Combine(_uploadPath, subtitle.StorageFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Subtitle file not found.");

            try
            {
                var plainTextContent = _parserService.GetPlainTextSubtitles(filePath);
                return Content(plainTextContent, "text/plain; charset=utf-8");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error parsing subtitle: {ex.Message}");
            }
        }


        [HttpPost("{id}/convert")]
        public IActionResult ConvertSubtitle(int id, [FromQuery] string targetFormat)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                return NotFound("Subtitle not found.");

            var sourceFilePath = Path.Combine(_uploadPath, subtitle.StorageFileName);

            if (!System.IO.File.Exists(sourceFilePath))
                return NotFound("Subtitle file not found.");

            try
            {
                var subtitles = _parserService.ParseSubtitle(sourceFilePath);

                var targetFileName = Path.GetFileNameWithoutExtension(subtitle.OriginalFileName) + "." + targetFormat;
                var targetFilePath = Path.Combine(_uploadPath, targetFileName);

                _parserService.WriteSubtitles(targetFilePath, subtitles, targetFormat);

                // _repository.AddConvertedSubtitle(subtitle.Id, targetFileName, targetFormat);

                return Ok($"Subtitle converted and saved as {targetFileName}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error converting subtitle: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error converting subtitle: {ex.Message}");
            }
        }

        [HttpGet("{id}/download/{format}")]
        public IActionResult DownloadConvertedSubtitle(int id, string format)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                return NotFound("Subtitle not found.");

            var sourceFilePath = Path.Combine(_uploadPath, subtitle.StorageFileName);

            if (!System.IO.File.Exists(sourceFilePath))
                return NotFound("Subtitle file not found.");

            try
            {
                var subtitles = _parserService.ParseSubtitle(sourceFilePath);

                var convertedFileName = Path.GetFileNameWithoutExtension(subtitle.OriginalFileName) + "." + format;
                var memoryStream = new MemoryStream();

                _parserService.WriteSubtitlesToStream(memoryStream, subtitles, format);
                memoryStream.Position = 0;

                return File(memoryStream, "application/octet-stream", convertedFileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error converting subtitle: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error converting subtitle: {ex.Message}");
            }
        }
    }
}