namespace Moshi.SubtitlesSite.Models;

public class SubtitleUploadModel
{
    public int ShowId { get; set; }
    public string Language { get; set; }
    public string Format { get; set; }
    public IFormFile File { get; set; }
}