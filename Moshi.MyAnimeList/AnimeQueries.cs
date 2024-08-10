namespace Moshi.MyAnimeList;

using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.MyAnimeList.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnimeQueries
{
    private readonly SqliteConnection _db;

    public AnimeQueries(SqliteConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<MoshiAnime>> GetAllAnimeAsync(int limit = 100)
    {
        await _db.OpenAsync();
        return await _db.QueryAsync<MoshiAnime>("SELECT * FROM Anime LIMIT @Limit", new { Limit = limit });
    }

    public async Task<MoshiAnime> GetAnimeByIdAsync(int id)
    {
        await _db.OpenAsync();
        return await _db.QueryFirstOrDefaultAsync<MoshiAnime>("SELECT * FROM Anime WHERE AnimeID = @Id", new { Id = id });
    }

    public async Task<IEnumerable<MoshiAnime>> SearchAnimeAsync(string title, int limit = 100)
    {
        await _db.OpenAsync();
        return await _db.QueryAsync<MoshiAnime>("SELECT * FROM Anime WHERE Title LIKE @Title LIMIT @Limit", new { Title = $"%{title}%", Limit = limit });
    }

    public async Task<AnimeWithRelatedData> GetAnimeWithRelatedDataAsync(int id)
    {
        await _db.OpenAsync();
        var sql = @"
    SELECT * FROM Anime WHERE AnimeID = @Id;
    SELECT s.Season, s.Year FROM AnimeSeason AS1
    JOIN Season s ON AS1.SeasonID = s.SeasonID
    WHERE AS1.AnimeID = @Id;
    SELECT URL FROM Sources WHERE AnimeID = @Id;
    SELECT Synonym FROM Synonyms WHERE AnimeID = @Id;
    SELECT RelatedAnimeURL FROM RelatedAnime WHERE AnimeID = @Id;
    SELECT Tag FROM Tags WHERE AnimeID = @Id;";

        using var multi = await _db.QueryMultipleAsync(sql, new { Id = id });
        var animeData = await multi.ReadFirstOrDefaultAsync<MoshiAnime>();
        if (animeData == null) return null;

        var seasons = (await multi.ReadAsync<MoshiAnimeSeason>()).ToList();
        var sources = (await multi.ReadAsync<string>()).ToList();
        var synonyms = (await multi.ReadAsync<string>()).ToList();
        var relatedAnime = (await multi.ReadAsync<string>()).ToList();
        var tags = (await multi.ReadAsync<string>()).ToList();

        var anime = new AnimeWithRelatedData
        {
            AnimeID = animeData.AnimeID,
            Title = animeData.Title,
            Type = animeData.Type,
            Episodes = animeData.Episodes,
            Status = animeData.Status,
            Picture = animeData.Picture,
            Thumbnail = animeData.Thumbnail,
            Seasons = seasons.Any() ? seasons : null,
            Sources = sources.Any() ? sources : null,
            Synonyms = synonyms.Any() ? synonyms : null,
            RelatedAnime = relatedAnime.Any() ? relatedAnime : null,
            Tags = tags.Any() ? tags : null,
            AnimeSeason = seasons.Any() ? new MoshiAnimeSeason { Season = seasons.First().Season, Year = seasons.First().Year } : null
        };

        return anime;
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> GetAnimeBySeason(string season, int year, int limit = 100)
    {
        await _db.OpenAsync();
        var sql = @"
        SELECT a.*, s.Season, s.Year 
        FROM Anime a
        JOIN AnimeSeason AS1 ON a.AnimeID = AS1.AnimeID
        JOIN Season s ON AS1.SeasonID = s.SeasonID
        WHERE s.Season = @Season AND s.Year = @Year
        LIMIT @Limit";

        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Season = season, Year = year, Limit = limit });
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> SearchAnimeByTags(List<string> tags, int limit = 100)
    {
        await _db.OpenAsync();
        var sql = @"
        SELECT a.* 
        FROM Anime a
        JOIN Tags t ON a.AnimeID = t.AnimeID
        WHERE t.Tag IN @Tags
        GROUP BY a.AnimeID
        HAVING COUNT(DISTINCT t.Tag) = @TagCount
        LIMIT @Limit";

        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Tags = tags, TagCount = tags.Count, Limit = limit });
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> GetAnimeByType(string type, int limit = 100)
    {
        await _db.OpenAsync();
        var sql = "SELECT * FROM Anime WHERE Type = @Type LIMIT @Limit";

        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Type = type, Limit = limit });
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> GetAnimeByYearRange(int startYear, int endYear, int limit = 100)
    {
        await _db.OpenAsync();
        var sql = @"
        SELECT DISTINCT a.* 
        FROM Anime a
        JOIN AnimeSeason AS1 ON a.AnimeID = AS1.AnimeID
        JOIN Season s ON AS1.SeasonID = s.SeasonID
        WHERE s.Year BETWEEN @StartYear AND @EndYear
        LIMIT @Limit";

        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { StartYear = startYear, EndYear = endYear, Limit = limit });
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> GetAnimeByStatus(string status, int limit = 100)
    {
        await _db.OpenAsync();
        var sql = "SELECT * FROM Anime WHERE Status = @Status LIMIT @Limit";
        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Status = status, Limit = limit });
    }
    //public async Task<IEnumerable<AnimeWithRelatedData>> GetRelatedAnime(int animeId)
    //{
    //    await _db.OpenAsync();
    //    var sql = @"
    //    SELECT a.* 
    //    FROM Anime a
    //    JOIN RelatedAnime ra ON a.AnimeID = ra.AnimeID
    //    WHERE ra.RelatedAnimeURL LIKE @AnimeUrl";

    //    // First, get the URL of the anime we're looking for
    //    var animeUrlSql = "SELECT * FROM Anime WHERE AnimeID = @AnimeId";
    //    var anime = await _db.QueryFirstOrDefaultAsync<AnimeWithRelatedData>(animeUrlSql, new { AnimeId = animeId });

    //    if (anime == null)
    //        return new List<AnimeWithRelatedData>();

    //    // Now, search for anime that have this URL in their RelatedAnimeURL
    //    return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { AnimeUrl = $"%{anime.Sources.FirstOrDefault()}%" });
    //}

    public async Task<IEnumerable<AnimeWithRelatedData>> SearchAnimeBySynonym(string synonym)
    {
        await _db.OpenAsync();
        var sql = @"
            SELECT a.* 
            FROM Anime a
            JOIN Synonyms s ON a.AnimeID = s.AnimeID
            WHERE s.Synonym LIKE @Synonym";
        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Synonym = $"%{synonym}%" });
    }

    public async Task<IEnumerable<AnimeWithRelatedData>> GetAnimeWithMostEpisodes(int limit)
    {
        await _db.OpenAsync();
        var sql = "SELECT * FROM Anime ORDER BY Episodes DESC LIMIT @Limit";
        return await _db.QueryAsync<AnimeWithRelatedData>(sql, new { Limit = limit });
    }
}
