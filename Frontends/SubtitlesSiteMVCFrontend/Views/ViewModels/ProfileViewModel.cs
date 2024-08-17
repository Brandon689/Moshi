using Moshi.SubtitlesSite.Models;

namespace SubtitlesSiteMVCFrontend.Views.ViewModels;

public class ProfileViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public List<UserBadge> Badges { get; set; }
    public DateTime RegisteredDate { get; set; }
    public string RegisteredCountry { get; set; }
    public DateTime LastLoginDate { get; set; }
    public int UploadedSubtitlesCount { get; set; }
    public List<ProfileComment> Comments { get; set; }
}

//public class ProfileComment
//{
//    public string Username { get; set; }
//    public DateTime CommentDate { get; set; }
//    public string CommentText { get; set; }
//}