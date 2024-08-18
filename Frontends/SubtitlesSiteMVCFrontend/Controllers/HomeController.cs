using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Services;
using SubtitlesSiteMVCFrontend.Views.ViewModels;

namespace SubtitlesSiteMVCFrontend.Controllers;

public class HomeController : Controller
{
    private readonly SubtitleService _subtitleService;
    private readonly MoviesService _moviesService;
    private readonly SubtitleRequestService _requestService;

    public HomeController(SubtitleService subtitleService, MoviesService moviesService, SubtitleRequestService requestService)
    {
        _subtitleService = subtitleService;
        _moviesService = moviesService;
        _requestService = requestService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var viewModel = new HomePageViewModel
        {
            NewSubtitles = _subtitleService.GetNewSubtitles(5).ToList(),
            FeaturedSubtitles = _subtitleService.GetFeaturedSubtitles(5).ToList(),
            TopUploaders = _subtitleService.GetTopUploaders(5).ToList(),
            MostDownloaded = _subtitleService.GetMostDownloadedSubtitles(5).ToList(),
            RequestedSubtitles = _requestService.GetSubtitleRequests(5),
            LatestComments = _subtitleService.GetLatestComments(5).ToList(),
            PopularMovies = _moviesService.GetPopularMovies(5).ToList()
        };

        return View(viewModel);
    }
}