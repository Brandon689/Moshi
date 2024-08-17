namespace Moshi.SubtitlesSite.Models;

public class ProfileComment
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    public int CommenterId { get; set; }
    public string CommenterUsername { get; set; }
    public string CommentText { get; set; }
    public DateTime CommentDate { get; set; }
}