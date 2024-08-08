using Moshi.Forums.Services;
using Moshi.Forums.Models.DTOs;

namespace Moshi.Forums.Extensions;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this WebApplication app)
    {
        app.MapGet("/profile/{userId}", async (int userId, ProfileService profileService) =>
        {
            var profile = await profileService.GetUserProfile(userId);
            return profile != null ? Results.Ok(profile) : Results.NotFound();
        }).RequireAuthorization();

        app.MapPut("/profile/{userId}", async (int userId, UserProfileUpdateDto updateDto, ProfileService profileService) =>
        {
            try
            {
                await profileService.UpdateUserProfile(userId, updateDto);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        }).RequireAuthorization();
    }
}
