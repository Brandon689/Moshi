using Microsoft.AspNetCore.Mvc;

namespace SubtitlesSiteMVCFrontend.Controllers;
public class MovieController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
