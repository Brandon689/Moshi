namespace Moshi.SubtitlesSite.Models
{
    public class SubtitleUpload
    {
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public string ReleaseInfo { get; set; }
        public IFormFile File { get; set; }
        public double? FPS { get; set; }
        public int NumDiscs { get; set; } = 1;
        public string Notes { get; set; }
    }
}