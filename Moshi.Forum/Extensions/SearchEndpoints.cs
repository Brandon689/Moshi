using Moshi.Forums.Services;

namespace Moshi.Forums.Extensions;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        app.MapGet("/search", async (string query, SearchService searchService) =>
        {
            var results = await searchService.Search(query);
            return Results.Ok(results);
        }).RequireAuthorization();
    }
}
