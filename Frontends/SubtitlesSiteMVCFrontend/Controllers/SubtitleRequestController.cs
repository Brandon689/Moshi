using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Models;
using Moshi.SubtitlesSite.Services;

namespace SubtitlesSiteMVCFrontend.Controllers;

[Route("Request")]
public class SubtitleRequestController : Controller
{
    private readonly SubtitleRequestService _requestService;

    public SubtitleRequestController(SubtitleRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var requests = _requestService.GetAllRequests();
        return View(requests);
    }

    [HttpGet("Detail/{id?}")]  // This will match /Request/Detail or /Request/Detail/{id}
    public IActionResult Detail(int id = 0)
    {
        SubtitleRequest s = new();
        s.LatestUploadDate = DateTime.Now;
        s.Id = id;
        s.SubtitleCount = 100;
        s.LatestSubtitleLanguage = "en";
        s.Year = 1222;
        s.MovieName = "howls moving castle";
        s.Rating = 9.3f;
        return View(s);
    }
}
