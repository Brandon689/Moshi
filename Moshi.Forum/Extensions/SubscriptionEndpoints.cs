using Moshi.Forums.Services;
using System.Security.Claims;

namespace Moshi.Forums.Extensions;

public static class SubscriptionEndpoints
{
    public static void MapSubscriptionEndpoints(this WebApplication app)
    {
        app.MapPost("/subscriptions/{forumId}", async (int forumId, SubscriptionService subscriptionService, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await subscriptionService.SubscribeToForum(userId, forumId);
            return Results.Ok();
        }).RequireAuthorization();

        app.MapDelete("/subscriptions/{forumId}", async (int forumId, SubscriptionService subscriptionService, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await subscriptionService.UnsubscribeFromForum(userId, forumId);
            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/subscriptions", async (SubscriptionService subscriptionService, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var subscriptions = await subscriptionService.GetUserSubscriptions(userId);
            return Results.Ok(subscriptions);
        }).RequireAuthorization();
    }
}
