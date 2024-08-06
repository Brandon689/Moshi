using Moshi.Forums.Models;
using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users", async (UserService service) =>
            await service.GetAllUsersAsync())
        .WithName("GetAllUsers")
        .WithOpenApi();

        app.MapGet("/api/users/{id}", async (int id, UserService service) =>
            await service.GetUserByIdAsync(id) is User user
                ? Results.Ok(user)
                : Results.NotFound())
        .WithName("GetUserById")
        .WithOpenApi();

        app.MapPost("/api/users", async (User user, UserService service) =>
        {
            var id = await service.CreateUserAsync(user);
            return Results.Created($"/api/users/{id}", user);
        })
        .WithName("CreateUser")
        .WithOpenApi();

        app.MapPut("/api/users/{id}", async (int id, User user, UserService service) =>
        {
            var updatedUser = await service.UpdateUserAsync(id, user);
            return updatedUser is not null ? Results.Ok(updatedUser) : Results.NotFound();
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        app.MapDelete("/api/users/{id}", async (int id, UserService service) =>
        {
            var result = await service.DeleteUserAsync(id);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}
