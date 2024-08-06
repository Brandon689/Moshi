using Moshi.Forums.Models;
using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;

public static class ThreadEndpoints
{
    public static void MapThreadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/threads", async (ThreadService service) =>
            await service.GetAllThreadsAsync())
        .WithName("GetAllThreads")
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
    }
}
