using Moshi.Forums.Services;
using System.Security.Claims;

namespace Moshi.Forums.Extensions;

public static class ModerationEndpoints
{
    public static void MapModerationEndpoints(this WebApplication app)
    {
        //app.MapPost("/moderation/lock-thread/{threadId}", async (int threadId, ModerationService moderationService, HttpContext context) =>
        //{
        //    var moderatorId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var result = await moderationService.LockThread(threadId, moderatorId);
        //    return result ? Results.Ok() : Results.BadRequest(new { Message = "Failed to lock thread" });
        //}).RequireAuthorization(policy => policy.RequireRole("Moderator", "Admin"));

        //app.MapPost("/moderation/delete-post/{postId}", async (int postId, ModerationService moderationService, HttpContext context) =>
        //{
        //    var moderatorId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var result = await moderationService.DeletePost(postId, moderatorId);
        //    return result ? Results.Ok() : Results.BadRequest(new { Message = "Failed to delete post" });
        //}).RequireAuthorization(policy => policy.RequireRole("Moderator", "Admin"));

        //app.MapPost("/moderation/ban-user/{userId}", async (int userId, DateTime banEndDate, ModerationService moderationService, HttpContext context) =>
        //{
        //    var moderatorId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var result = await moderationService.BanUser(userId, moderatorId, banEndDate);
        //    return result ? Results.Ok() : Results.BadRequest(new { Message = "Failed to ban user" });
        //}).RequireAuthorization(policy => policy.RequireRole("Moderator", "Admin"));
    }
}
