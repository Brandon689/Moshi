using Microsoft.AspNetCore.Mvc;
using Moshi.MediaWiki.Models;
using Moshi.MediaWiki.Services;

namespace Moshi.MediaWiki.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        try
        {
            _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}