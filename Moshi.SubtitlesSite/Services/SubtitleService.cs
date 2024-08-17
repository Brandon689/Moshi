using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;

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

        public Subtitle GetSubtitleById(int id)
        {
            return _repository.GetSubtitleById(id);
        }

        public async Task<int> UploadSubtitle(SubtitleUpload model)
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
                MovieId = model.MovieId,
                UserId = model.UserId, // Assuming you've added this to the model
                Language = model.Language,
                Format = model.Format,
                ReleaseInfo = model.ReleaseInfo, // New field
                StorageFileName = storageFileName,
                OriginalFileName = originalFileName,
                UploadDate = DateTime.UtcNow,
                Downloads = 0,
                FPS = model.FPS, // New field
                NumDiscs = model.NumDiscs, // New field
                Notes = model.Notes // New field
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

        public bool UpdateSubtitle(int id, SubtitleUpdate model)
        {
            var subtitle = _repository.GetSubtitleById(id);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            subtitle.Language = model.Language;
            subtitle.Format = model.Format;
            subtitle.ReleaseInfo = model.ReleaseInfo;
            subtitle.FPS = model.FPS;
            subtitle.NumDiscs = model.NumDiscs;
            subtitle.Notes = model.Notes;

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

        public bool RateSubtitle(int subtitleId, int userId, float rating)
        {
            if (rating < 1 || rating > 10)
                throw new ArgumentException("Rating must be between 1 and 10");

            var subtitle = _repository.GetSubtitleById(subtitleId);
            if (subtitle == null)
                throw new KeyNotFoundException("Subtitle not found.");

            return _repository.RateSubtitle(subtitleId, userId, rating);
        }

        public IEnumerable<Subtitle> GetSubtitlesByMovieId(int movieId)
        {
            return _repository.GetSubtitlesByMovieId(movieId);
        }

        public IEnumerable<Subtitle> SearchSubtitles(string query)
        {
            return _repository.SearchSubtitles(query);
        }

        public IEnumerable<Subtitle> GetTopRatedSubtitles(int limit)
        {
            return _repository.GetTopRatedSubtitles(limit);
        }

        public bool AddComment(SubtitleComment model)
        {
            var comment = new SubtitleComment
            {
                SubtitleId = model.SubtitleId,
                UserId = model.UserId,
                Comment = model.Comment,
                CommentDate = DateTime.UtcNow
            };
            return _repository.AddSubtitleComment(comment);
        }

        public IEnumerable<SubtitleComment> GetComments(int subtitleId)
        {
            return _repository.GetSubtitleComments(subtitleId);
        }

        public IEnumerable<SubtitleCommentWithUsername> GetCommentsWithUsernames(int movieId)
        {
            var commentsWithUsernames = _repository.GetCommentsWithUsernames(movieId);
            return commentsWithUsernames;
        }

        public int GetCommentsCount(int subtitleId)
        {
            return _repository.GetSubtitleCommentCount(subtitleId);
        }

        public double GetAverageRating(int subtitleId)
        {
            return _repository.GetAverageRating(subtitleId);
        }



        public IEnumerable<SubtitleWithMovieDetails> GetNewSubtitles(int count)
        {
            return _repository.GetSubtitlesWithMovieDetails(count, "UploadDate");
        }

        public IEnumerable<SubtitleWithMovieDetails> GetMostDownloadedSubtitles(int count)
        {
            return _repository.GetSubtitlesWithMovieDetails(count, "Downloads");
        }

        public IEnumerable<SubtitleWithMovieDetails> GetFeaturedSubtitles(int count)
        {
            // Assuming you want to feature top-rated subtitles
            return _repository.GetSubtitlesWithMovieDetails(count, "ImdbRating");
        }




        public IEnumerable<(string Username, int UploadCount, DateTime LatestUpload)> GetTopUploaders(int count)
        {
            return _repository.GetTopUploaders(count);
        }


        //public IEnumerable<(string MovieName, int RequestCount, DateTime LatestRequestDate)> GetMostRequestedSubtitles(int count)
        //{
        //    return _repository.GetMostRequestedSubtitles(count);
        //}

        public IEnumerable<SubtitleCommentWithUsername> GetLatestComments(int count)
        {
            return _repository.GetLatestComments(count);
        }


    }
}