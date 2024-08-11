using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Moshi.MyAnimeList;
using Moshi.MyAnimeList.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AnimeDatabase as a singleton
builder.Services.AddSingleton<AnimeDatabase>(sp =>
{
    var dbPath = builder.Configuration.GetValue<string>("DatabasePath") ?? "anime.db";
    return new AnimeDatabase($"Data Source={dbPath}");
});

// Register SqliteConnection as a scoped service
builder.Services.AddScoped<SqliteConnection>(sp =>
{
    var dbPath = builder.Configuration.GetValue<string>("DatabasePath") ?? "anime.db";
    return new SqliteConnection($"Data Source={dbPath}");
});

// Register AnimeQueries as a scoped service
builder.Services.AddScoped<AnimeQueries>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Define API endpoints
app.MapGet("/", () => "Welcome to the Anime Database API!");

app.MapPost("/import", async (AnimeDatabase db, IConfiguration config) =>
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    var jsonPath = config.GetValue<string>("JsonFilePath") ?? "anime-offline-database.json";
    db.ImportFromJson(jsonPath);
    stopwatch.Stop();
    Console.WriteLine($"Time taken: {stopwatch.Elapsed.TotalMilliseconds} ms");
    return Results.Ok("Import completed successfully");
});

app.MapGet("/anime", async (AnimeQueries queries) =>
{
    IEnumerable<MoshiAnime> anime = await queries.GetAllAnimeAsync();
    return Results.Ok(anime);
});

app.MapGet("/anime/{id}", async Task<Results<Ok<MoshiAnime>, NotFound>> (int id, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeByIdAsync(id);
    return anime is null ? TypedResults.NotFound() : TypedResults.Ok(anime);
})
.WithName("GetAnimeById")
.WithTags("Anime")
.WithOpenApi();


app.MapGet("/anime/search", async Task<Ok<IEnumerable<MoshiAnime>>> ([AsParameters] AnimeSearchParams searchParams, AnimeQueries queries) =>
{
    var anime = await queries.SearchAnimeAsync(searchParams.Title);
    return TypedResults.Ok(anime);
})
.WithName("SearchAnime")
.WithTags("Anime")
.WithOpenApi();

app.MapGet("/anime/{id}/full", async (int id, AnimeQueries queries) =>
{
    AnimeWithRelatedData animeWithRelatedData = await queries.GetAnimeWithRelatedDataAsync(id);
    if (animeWithRelatedData == null)
        return Results.NotFound();
    return Results.Ok(animeWithRelatedData);
});

app.MapPost("/vacuum", async (AnimeDatabase db) =>
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    db.VacuumDatabase();

    stopwatch.Stop();
    return Results.Ok($"Vacuum completed successfully. Time taken: {stopwatch.Elapsed.TotalMilliseconds} ms");
});

app.MapPost("/create-indexes", (AnimeDatabase db) =>
{
    db.CreateIndexes();
    return Results.Ok("Indexes created successfully");
});

app.MapGet("/anime/season/{season}/{year}", async Task<Ok<IEnumerable<AnimeWithRelatedData>>> (string season, int year, int? limit, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeBySeason(season, year, limit ?? 100);
    return TypedResults.Ok(anime);
})
.WithName("GetAnimeBySeason")
.WithTags("Anime")
.WithOpenApi();

app.MapGet("/anime/tags", async Task<Ok<IEnumerable<AnimeWithRelatedData>>> ([AsParameters] AnimeTagsParams tagsParams, AnimeQueries queries) =>
{
    var tagList = tagsParams.Tags.Split(',').ToList();
    var anime = await queries.SearchAnimeByTags(tagList, tagsParams.Limit ?? 100);
    return TypedResults.Ok(anime);
})
.WithName("SearchAnimeByTags")
.WithTags("Anime")
.WithOpenApi();

app.MapGet("/anime/type/{type}", async (string type, int? limit, AnimeQueries queries) =>
{
    IEnumerable<AnimeWithRelatedData> anime = await queries.GetAnimeByType(type, limit ?? 100);
    return Results.Ok(anime);
});

app.MapGet("/anime/years", async (int startYear, int endYear, int? limit, AnimeQueries queries) =>
{
    IEnumerable<AnimeWithRelatedData> anime = await queries.GetAnimeByYearRange(startYear, endYear, limit ?? 100);
    return Results.Ok(anime);
});

// Get Anime by Status
app.MapGet("/anime/status/{status}", async (string status, int? limit, AnimeQueries queries) =>
{
    try
    {
        IEnumerable<AnimeWithRelatedData> anime = await queries.GetAnimeByStatus(status, limit ?? 100);
        if (!anime.Any())
            return Results.NotFound($"No anime found with status '{status}'");
        return Results.Ok(anime);
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});

// Get Related Anime
//app.MapGet("/anime/{id}/related", async (int id, AnimeQueries queries) =>
//{
//    var relatedAnime = await queries.GetRelatedAnime(id);
//    return Results.Ok(relatedAnime);
//});

// Search Anime by Synonym
app.MapGet("/anime/synonym/{synonym}", async (string synonym, AnimeQueries queries) =>
{
    var anime = await queries.SearchAnimeBySynonym(synonym);
    return Results.Ok(anime);
});

// Get Anime with Most Episodes
app.MapGet("/anime/most-episodes", async (int limit, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeWithMostEpisodes(limit);
    return Results.Ok(anime);
});

app.Run();
public record AnimeSearchParams(string Title);
public record AnimeTagsParams(string Tags, int? Limit);
