using Moshi.Blog.Models;
using Moshi.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Moshi.Blog.Extensions
{
    public static class PostEndpoints
    {
        public static void MapPostEndpoints(this WebApplication app)
        {
            app.MapGet("/api/posts", async (
                PostService postService,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10) =>
            {
                if (page < 1 || pageSize < 1 || pageSize > 100)
                {
                    return Results.BadRequest("Invalid pagination parameters");
                }

                var (posts, totalCount) = await postService.GetPaginatedPosts(page, pageSize);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new
                {
                    Posts = posts,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = page > 1,
                    HasNextPage = page < totalPages
                };

                return Results.Ok(result);
            });

            app.MapGet("/api/posts/{id}", async (int id, PostService postService) =>
                await postService.GetPostById(id) is Post post
                    ? Results.Ok(post)
                    : Results.NotFound());

            app.MapPost("/api/posts", async (Post post, PostService postService) =>
            {
                var newPost = await postService.CreatePost(post);
                return Results.Created($"/api/posts/{newPost.Id}", newPost);
            });

            app.MapPut("/api/posts/{id}", async (int id, Post post, PostService postService) =>
            {
                var updatedPost = await postService.UpdatePost(id, post);
                return updatedPost is not null ? Results.Ok(updatedPost) : Results.NotFound();
            });

            app.MapDelete("/api/posts/{id}", async (int id, PostService postService) =>
                await postService.DeletePost(id) ? Results.Ok() : Results.NotFound());
        }
    }
}
