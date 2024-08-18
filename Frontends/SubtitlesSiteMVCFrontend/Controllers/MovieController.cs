using Microsoft.AspNetCore.Mvc;

namespace SubtitlesSiteMVCFrontend.Controllers;

[Route("Movie")]
public class MovieController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
}
