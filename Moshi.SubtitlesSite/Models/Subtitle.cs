namespace Moshi.SubtitlesSite.Models;

public class Subtitle
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public string Language { get; set; }
    public string Format { get; set; }
    public string StorageFileName { get; set; }
    public string OriginalFileName { get; set; }
    public DateTime UploadDate { get; set; }
    public int Downloads { get; set; }
}