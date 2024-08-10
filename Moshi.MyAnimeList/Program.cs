using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using Moshi.MyAnimeList;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
    var anime = await queries.GetAllAnimeAsync();
    return Results.Ok(anime);
});

app.MapGet("/anime/{id}", async (int id, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeByIdAsync(id);
    if (anime == null)
        return Results.NotFound();
    return Results.Ok(anime);
});

app.MapGet("/anime/search", async (string title, AnimeQueries queries) =>
{
    var anime = await queries.SearchAnimeAsync(title);
    return Results.Ok(anime);
});

app.MapGet("/anime/{id}/full", async (int id, AnimeQueries queries) =>
{
    var animeWithRelatedData = await queries.GetAnimeWithRelatedDataAsync(id);
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

// ... (existing code)

// Get Anime by Season
app.MapGet("/anime/season/{season}/{year}", async (string season, int year, int? limit, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeBySeason(season, year, limit ?? 100);
    return Results.Ok(anime);
});

app.MapGet("/anime/tags", async (string tags, int? limit, AnimeQueries queries) =>
{
    var tagList = tags.Split(',').ToList();
    var anime = await queries.SearchAnimeByTags(tagList, limit ?? 100);
    return Results.Ok(anime);
});

app.MapGet("/anime/type/{type}", async (string type, int? limit, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeByType(type, limit ?? 100);
    return Results.Ok(anime);
});

app.MapGet("/anime/years", async (int startYear, int endYear, int? limit, AnimeQueries queries) =>
{
    var anime = await queries.GetAnimeByYearRange(startYear, endYear, limit ?? 100);
    return Results.Ok(anime);
});

// Get Anime by Status
app.MapGet("/anime/status/{status}", async (string status, int? limit, AnimeQueries queries) =>
{
    try
    {
        var anime = await queries.GetAnimeByStatus(status, limit ?? 100);
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

// ... (existing code)


app.Run();
