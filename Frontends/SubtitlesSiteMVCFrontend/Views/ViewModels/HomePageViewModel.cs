using Moshi.SubtitlesSite.Models;

namespace SubtitlesSiteMVCFrontend.Views.ViewModels;

public class HomePageViewModel
{
    public List<SubtitleWithMovieDetails> NewSubtitles { get; set; }
    public List<SubtitleWithMovieDetails> FeaturedSubtitles { get; set; }
    public List<UploaderStats> TopUploaders { get; set; }
    public List<SubtitleWithMovieDetails> MostDownloaded { get; set; }
    //public List<ForumTopic> LatestForumTopics { get; set; }
    //public List<BlogArticle> LatestBlogArticles { get; set; }
    //public List<SubtitleRequest> RequestedSubtitles { get; set; }
    public List<SubtitleCommentWithUsername> LatestComments { get; set; }
    public List<Movie> PopularMovies { get; set; }
}


public class UploaderStats
{
    public string Username { get; set; }
    public int UploadCount { get; set; }
    public DateTime LatestUpload { get; set; }
}