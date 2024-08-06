using Moshi.Forums.Models;
using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;
public static class ForumEndpoints
{
    public static void MapForumEndpoints(this WebApplication app)
    {
        app.MapGet("/api/forums", async (ForumService service) =>
            await service.GetAllForumsAsync())
        .WithName("GetAllForums")
        .WithOpenApi();

        app.MapGet("/api/forums/{id}", async (int id, ForumService service) =>
            await service.GetForumByIdAsync(id) is Forum forum
                ? Results.Ok(forum)
                : Results.NotFound())
        .WithName("GetForumById")
        .WithOpenApi();

        app.MapPost("/api/forums", async (Forum forum, ForumService service) =>
        {
            var id = await service.CreateForumAsync(forum);
            return Results.Created($"/api/forums/{id}", forum);
        })
        .WithName("CreateForum")
        .WithOpenApi();

        app.MapPut("/api/forums/{id}", async (int id, Forum forum, ForumService service) =>
        {
            var updatedForum = await service.UpdateForumAsync(id, forum);
            return updatedForum is not null ? Results.Ok(updatedForum) : Results.NotFound();
        })
        .WithName("UpdateForum")
        .WithOpenApi();

        app.MapDelete("/api/forums/{id}", async (int id, ForumService service) =>
        {
            var result = await service.DeleteForumAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteForum")
        .WithOpenApi();
    }
}
