using Moshi.Forums.Services;
using System.Security.Claims;

namespace Moshi.Forums.Extensions;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        app.MapGet("/notifications", async (NotificationService notificationService, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var notifications = await notificationService.GetUserNotifications(userId);
            return Results.Ok(notifications);
        }).RequireAuthorization();

        app.MapPost("/notifications/{notificationId}/mark-as-read", async (int notificationId, NotificationService notificationService) =>
        {
            await notificationService.MarkNotificationAsRead(notificationId);
            return Results.Ok();
        }).RequireAuthorization();
    }
}
