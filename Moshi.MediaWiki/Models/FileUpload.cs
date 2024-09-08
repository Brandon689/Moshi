namespace Moshi.MediaWiki.Models;

public class FileUpload
{
    public int FileId { get; set; }
    public string Filename { get; set; }
    public string FilePath { get; set; }
    public string MimeType { get; set; }
    public DateTime UploadDate { get; set; }
    public int UserId { get; set; }
}
