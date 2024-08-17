namespace Moshi.SubtitlesSite.Services
{
    public class SubtitleUpdate
    {
        public string Language { get; set; }
        public string Format { get; set; }
        public string ReleaseInfo { get; set; }
        public double? FPS { get; set; }
        public int NumDiscs { get; set; }
        public string Notes { get; set; }
    }
}