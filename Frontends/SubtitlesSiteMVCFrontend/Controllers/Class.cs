using Moshi.SubtitlesSite.Services;
using Moshi.SubtitlesSite.Models;
using SubtitlesSiteMVCFrontend.Views.ViewModels;

namespace SubtitlesSiteMVCFrontend.Services
{
    public class HomePageService
    {
        private readonly SubtitleService _subtitleService;
        private readonly MoviesService _moviesService;

        public HomePageService(SubtitleService subtitleService, MoviesService moviesService)
        {
            _subtitleService = subtitleService;
            _moviesService = moviesService;
        }

        public HomePageViewModel GetHomePageData()
        {
            return new HomePageViewModel
            {
                NewSubtitles = _subtitleService.GetNewSubtitles(5).ToList(),
                FeaturedSubtitles = _subtitleService.GetFeaturedSubtitles(5).ToList(),
                TopUploaders = MapToUploaderStats(_subtitleService.GetTopUploaders(5)),
                MostDownloaded = _subtitleService.GetMostDownloadedSubtitles(5).ToList(),
                //RequestedSubtitles = MapToSubtitleRequests(_subtitleService.GetMostRequestedSubtitles(5)),
                LatestComments = _subtitleService.GetLatestComments(5).ToList(),
                PopularMovies = _moviesService.GetPopularMovies(10).ToList()
            };
        }

        private List<UploaderStats> MapToUploaderStats(IEnumerable<(string Username, int UploadCount, DateTime LatestUpload)> uploaders)
        {
            return uploaders.Select(u => new UploaderStats
            {
                Username = u.Username,
                UploadCount = u.UploadCount,
                LatestUpload = u.LatestUpload
            }).ToList();
        }

        //private List<SubtitleRequest> MapToSubtitleRequests(IEnumerable<(string MovieName, int RequestCount, DateTime LatestRequestDate)> requests)
        //{
        //    return requests.Select(r => new SubtitleRequest
        //    {
        //        MovieName = r.MovieName,
        //        RequestCount = r.RequestCount,
        //        LatestRequestDate = r.LatestRequestDate
        //    }).ToList();
        //}
    }
}