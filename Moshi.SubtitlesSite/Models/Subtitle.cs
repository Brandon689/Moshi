namespace Moshi.SubtitlesSite.Models;

public class Subtitle
{
    public int SubtitleId { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public string Language { get; set; }
    public string Format { get; set; }
    public string ReleaseInfo { get; set; }
    public string StorageFileName { get; set; }
    public string OriginalFileName { get; set; }
    public DateTime UploadDate { get; set; }
    public int Downloads { get; set; }
    public double? FPS { get; set; }
    public int NumDiscs { get; set; } = 1;
    public string Notes { get; set; }
}