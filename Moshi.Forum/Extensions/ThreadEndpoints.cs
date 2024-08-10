using Microsoft.AspNetCore.Mvc;
using Moshi.Forums.Models;
using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;

public static class ThreadEndpoints
{
    public static void MapThreadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/threads", async ([FromQuery] int? forumId, ThreadService service) =>
        {
            if (forumId.HasValue)
            {
                var threads = await service.GetThreadsByForumIdAsync(forumId.Value);
                return Results.Ok(threads);
            }
            else
            {
                var allThreads = await service.GetAllThreadsAsync();
                return Results.Ok(allThreads);
            }
        })
       .WithName("GetThreads")
       .WithOpenApi();

        app.MapGet("/api/threads/{id}", async (int id, ThreadService service) =>
            await service.GetThreadByIdAsync(id) is ForumThread thread
                ? Results.Ok(thread)
                : Results.NotFound())
        .WithName("GetThreadById")
        .WithOpenApi();

        app.MapPost("/api/threads", async (ForumThread thread, ThreadService service) =>
        {
            var id = await service.CreateThreadAsync(thread);
            return Results.Created($"/api/threads/{id}", thread);
        })
        .WithName("CreateThread")
        .WithOpenApi();

        app.MapPut("/api/threads/{id}", async (int id, ForumThread thread, ThreadService service) =>
        {
            var updatedThread = await service.UpdateThreadAsync(id, thread);
            return updatedThread is not null ? Results.Ok(updatedThread) : Results.NotFound();
        })
        .WithName("UpdateThread")
        .WithOpenApi();

        app.MapDelete("/api/threads/{id}", async (int id, ThreadService service) =>
        {
            var result = await service.DeleteThreadAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteThread")
        .WithOpenApi();

        //app.MapPost("/threads/{threadId}/lock", async (int threadId, ThreadService threadService, HttpContext context) =>
        //{
        //    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    var result = await threadService.LockThread(threadId, userId);
        //    return result ? Results.Ok() : Results.BadRequest(new { Message = "Failed to lock thread" });
        //}).RequireAuthorization(policy => policy.RequireRole("Moderator", "Admin"));
    }
}
