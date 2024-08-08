using Moshi.Forums.Models;
using Moshi.Forums.Services;
using Microsoft.AspNetCore.Mvc;

namespace Moshi.Forums.Extensions;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        app.MapGet("/api/posts", async ([FromQuery] int? threadId, PostService service) =>
        {
            if (threadId.HasValue)
            {
                var posts = await service.GetPostsByThreadIdAsync(threadId.Value);
                return Results.Ok(posts);
            }
            else
            {
                var allPosts = await service.GetAllPostsAsync();
                return Results.Ok(allPosts);
            }
        })
        .WithName("GetPosts")
        .WithOpenApi();

        app.MapGet("/api/posts/{id}", async (int id, PostService service) =>
            await service.GetPostByIdAsync(id) is Post post
                ? Results.Ok(post)
                : Results.NotFound())
        .WithName("GetPostById")
        .WithOpenApi();

        app.MapPost("/api/posts", async (Post post, PostService service) =>
        {
            try
            {
                Console.WriteLine(post);
                var createdPost = await service.CreatePostAsync(post);
                return Results.Created($"/api/posts/{createdPost.Id}", createdPost);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Results.StatusCode(500);
            }
        })
        .RequireAuthorization()
        .WithName("CreatePost")
        .WithOpenApi();

        app.MapPut("/api/posts/{id}", async (int id, Post post, PostService service) =>
        {
            var updatedPost = await service.UpdatePostAsync(id, post);
            return updatedPost is not null ? Results.Ok(updatedPost) : Results.NotFound();
        })
        .WithName("UpdatePost")
        .WithOpenApi();

        app.MapDelete("/api/posts/{id}", async (int id, PostService service) =>
        {
            var result = await service.DeletePostAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeletePost")
        .WithOpenApi();
    }
}
