using Moshi.SubtitlesSite.Models;
using SubtitlesSite.Data;

namespace Moshi.SubtitlesSite.Services
{
    public class SubtitleService
    {
        private readonly SubtitleRepository _repository;
        private readonly string _uploadPath;

        public SubtitleService(SubtitleRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _uploadPath = Path.Combine(env.ContentRootPath, "Uploads");
        }

        public async Task<int> UploadSubtitle(SubtitleUploadModel model)
        {
            var originalFileName = model.File.FileName;
            var storageFileName = Path.GetRandomFileName() + Path.GetExtension(originalFileName);
            var filePath = Path.Combine(_uploadPath, storageFileName);

            Directory.CreateDirectory(_uploadPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            var subtitle = new Subtitle
            {
                ShowId = model.ShowId,
                Language = model.Language,
                Format = model.Format,
                StorageFileName = storageFileName,
                OriginalFileName = originalFileName,
                UploadDate = DateTime.UtcNow,
                Downloads = 0
            };

            return _repository.CreateSubtitle(subtitle);
        }

        public (string FilePath, string ContentType, string FileName) PrepareSubtitleDownload(int id)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            var filePath = Path.Combine(_uploadPath, subtitle.StorageFileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Subtitle file not found.");

            _repository.IncrementDownloadCount(id);

            var contentType = "application/octet-stream";
            var fileExtension = Path.GetExtension(subtitle.StorageFileName).ToLowerInvariant();
            if (fileExtension == ".srt")
                contentType = "application/x-subrip";
            else if (fileExtension == ".vtt")
                contentType = "text/vtt";

            return (filePath, contentType, subtitle.OriginalFileName);
        }

        public bool UpdateSubtitle(int id, SubtitleUpdateModel model)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            subtitle.Language = model.Language;
            subtitle.Format = model.Format;

            return _repository.UpdateSubtitle(subtitle);
        }

        public bool DeleteSubtitle(int id)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            var success = _repository.DeleteSubtitle(id);
            if (success)
            {
                var filePath = Path.Combine(_uploadPath, subtitle.StorageFileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return success;
        }

        public bool RateSubtitle(int id, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            return _repository.RateSubtitle(id, rating);
        }

        public IEnumerable<Subtitle> GetSubtitlesByShowId(int showId)
        {
            return _repository.GetSubtitlesByShowId(showId);
        }

        public IEnumerable<Subtitle> SearchSubtitles(string query)
        {
            return _repository.SearchSubtitles(query);
        }

        public IEnumerable<Subtitle> GetTopRatedSubtitles(int limit)
        {
            return _repository.GetTopRatedSubtitles(limit);
        }
    }
}
