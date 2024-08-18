namespace Moshi.SubtitlesSite.Models;

public class UploaderStats
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public int UploadCount { get; set; }
    public DateTime LatestUpload { get; set; }
}