using Moshi.Blog.Models;
using Moshi.Blog.Services;

namespace Moshi.Blog.Extensions
{
    public static class CommentEndpoints
    {
        public static void MapCommentEndpoints(this WebApplication app)
        {
            app.MapGet("/api/posts/{postId}/comments", async (int postId, CommentService commentService) =>
                await commentService.GetCommentsByPostId(postId));

            app.MapPost("/api/comments", async (Comment comment, CommentService commentService) =>
            {
                var newComment = await commentService.CreateComment(comment);
                return Results.Created($"/api/comments/{newComment.Id}", newComment);
            });

            app.MapPut("/api/comments/{id}", async (int id, Comment comment, CommentService commentService) =>
            {
                var updatedComment = await commentService.UpdateComment(id, comment);
                return updatedComment is not null ? Results.Ok(updatedComment) : Results.NotFound();
            });

            app.MapDelete("/api/comments/{id}", async (int id, CommentService commentService) =>
                await commentService.DeleteComment(id) ? Results.Ok() : Results.NotFound());
        }
    }
}