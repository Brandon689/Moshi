using Moshi.Blog.Models;
using Moshi.Blog.Services;

namespace Moshi.Blog.Extensions
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapGet("/api/users", async (UserService userService) =>
                await userService.GetAllUsers());

            app.MapGet("/api/users/{id}", async (int id, UserService userService) =>
                await userService.GetUserById(id) is User user
                    ? Results.Ok(user)
                    : Results.NotFound());

            app.MapPost("/api/users", async (User user, UserService userService) =>
            {
                var newUser = await userService.CreateUser(user);
                return Results.Created($"/api/users/{newUser.Id}", newUser);
            });

            app.MapPut("/api/users/{id}", async (int id, User user, UserService userService) =>
            {
                var updatedUser = await userService.UpdateUser(id, user);
                return updatedUser is not null ? Results.Ok(updatedUser) : Results.NotFound();
            });

            app.MapDelete("/api/users/{id}", async (int id, UserService userService) =>
                await userService.DeleteUser(id) ? Results.Ok() : Results.NotFound());
        }
    }
}