using Moshi.Blog.Models;
using Moshi.Blog.Services;

namespace Moshi.Blog.Extensions
{
    public static class PostEndpoints
    {
        public static void MapPostEndpoints(this WebApplication app)
        {
            app.MapGet("/api/posts", async (PostService postService) =>
                await postService.GetAllPosts());

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