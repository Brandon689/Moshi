namespace Moshi.SubtitlesSite.Models;

public class SubtitleComment
{
    public int CommentId { get; set; }
    public int SubtitleId { get; set; }
    public int UserId { get; set; }
    public string Comment { get; set; }
    public DateTime CommentDate { get; set; }
}