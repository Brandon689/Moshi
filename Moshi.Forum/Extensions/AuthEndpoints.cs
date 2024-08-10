using Moshi.Forums.Models.DTOs;
using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (UserRegistrationDto registrationDto, AuthService authService) =>
        {
            try
            {
                var (token, userDto) = await authService.RegisterUser(registrationDto);
                return Results.Ok(new { Token = token, User = userDto });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });

        app.MapPost("/auth/login", async (UserLoginDto loginDto, AuthService authService) =>
        {
            try
            {
                var (token, userDto) = await authService.LoginUser(loginDto);
                return Results.Ok(new { Token = token, User = userDto });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        });
    }
}
