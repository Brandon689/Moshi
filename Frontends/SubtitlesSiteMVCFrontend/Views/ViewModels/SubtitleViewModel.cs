namespace SubtitlesSiteMVCFrontend.Views.ViewModels;

public class SubtitleViewModel
{
    public int SubtitleId { get; set; }
    public string MovieTitle { get; set; }
    public int MovieYear { get; set; }
    public string SubtitleName { get; set; }
    public string ReleaseInfo { get; set; }
    public string WatchOnlineLink { get; set; }
    public string SubtitleSearcherLink { get; set; }
    public int CdCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime UploadDate { get; set; }
    public int Downloads { get; set; }
    public double SubtitleRating { get; set; }
    public double MovieRating { get; set; }
    public string UploaderName { get; set; }
}
