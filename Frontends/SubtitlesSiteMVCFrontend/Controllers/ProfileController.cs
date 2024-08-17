using Microsoft.AspNetCore.Mvc;
using Moshi.SubtitlesSite.Services;
using SubtitlesSiteMVCFrontend.Views.ViewModels;

namespace SubtitlesSiteMVCFrontend.Controllers;

[Route("Profile")]
public class ProfileController : Controller
{
    private readonly UserService _userService;

    public ProfileController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public IActionResult Details(int userId)
    {
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            return View();
        }
        var viewModel = new ProfileViewModel
        {
            UserId = user.UserId,
            Username = user.Username,
            Badges = _userService.GetUserBadges(user.UserId).ToList(),
            RegisteredDate = user.RegistrationDate,
            RegisteredCountry = _userService.GetUserCountry(user.UserId),
            LastLoginDate = user.LastLoginDate ?? DateTime.MinValue,
            UploadedSubtitlesCount = _userService.GetUserUploadedSubtitlesCount(user.UserId),
            Comments = _userService.GetUserProfileComments(user.UserId).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int userId, string comment)
    {
        if (User.Identity.IsAuthenticated)
        {
            _userService.AddProfileComment(userId, User.Identity.Name, comment);
            return RedirectToAction("Index", new { username = _userService.GetUserById(userId).Username });
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }
}