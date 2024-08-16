using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System.Data;

namespace SubtitlesSite.Data;

public class SubtitleRepository
{
    private readonly string _connectionString;

    public SubtitleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    // Show-related methods
    public IEnumerable<ShowWithSubtitleCount> GetAllShows()
    {
        using var db = CreateConnection();
        var sql = @"
        SELECT s.*, 
               CAST(COUNT(sub.Id) AS INTEGER) as SubtitleCount 
        FROM Shows s 
        LEFT JOIN Subtitles sub ON s.Id = sub.ShowId 
        GROUP BY s.Id";

        return db.Query<ShowWithSubtitleCount>(sql);
    }



    public Show GetShowById(int id)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<Show>("SELECT * FROM Shows WHERE Id = @Id", new { Id = id });
    }

    public int CreateShow(Show show)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO Shows (Title, Year, Type, Description, Genre, Director, Cast, 
                NumberOfSeasons, NumberOfEpisodes, Language, Country, Rating, PosterUrl, 
                DateAdded, LastUpdated) 
                VALUES (@Title, @Year, @Type, @Description, @Genre, @Director, @Cast, 
                @NumberOfSeasons, @NumberOfEpisodes, @Language, @Country, @Rating, @PosterUrl, 
                @DateAdded, @LastUpdated);
                SELECT last_insert_rowid()";
        return db.ExecuteScalar<int>(sql, show);
    }

    // Subtitle-related methods
    public IEnumerable<Subtitle> GetSubtitlesByShowId(int showId)
    {
        using var db = CreateConnection();
        return db.Query<Subtitle>("SELECT * FROM Subtitles WHERE ShowId = @ShowId", new { ShowId = showId });
    }

    public Subtitle GetSubtitleById(int id)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<Subtitle>("SELECT * FROM Subtitles WHERE Id = @Id", new { Id = id });
    }

    public int CreateSubtitle(Subtitle subtitle)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO Subtitles (ShowId, Language, Format, StorageFileName, OriginalFileName, UploadDate, Downloads) 
                    VALUES (@ShowId, @Language, @Format, @StorageFileName, @OriginalFileName, @UploadDate, @Downloads);
                    SELECT last_insert_rowid()";
        return db.ExecuteScalar<int>(sql, subtitle);
    }

    public void IncrementDownloadCount(int subtitleId)
    {
        using var db = CreateConnection();
        db.Execute("UPDATE Subtitles SET Downloads = Downloads + 1 WHERE Id = @Id", new { Id = subtitleId });
    }

    // New method to update a subtitle
    public bool UpdateSubtitle(Subtitle subtitle)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE Subtitles 
                    SET ShowId = @ShowId, Language = @Language, Format = @Format, 
                        StorageFileName = @StorageFileName, OriginalFileName = @OriginalFileName, 
                        UploadDate = @UploadDate, Downloads = @Downloads
                    WHERE Id = @Id";
        var rowsAffected = db.Execute(sql, subtitle);
        return rowsAffected > 0;
    }

    // New method to delete a subtitle
    public bool DeleteSubtitle(int id)
    {
        using var db = CreateConnection();
        var rowsAffected = db.Execute("DELETE FROM Subtitles WHERE Id = @Id", new { Id = id });
        return rowsAffected > 0;
    }

    public bool UpdateShow(Show show)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE Shows 
                SET Title = @Title, Year = @Year, Type = @Type, Description = @Description, 
                Genre = @Genre, Director = @Director, Cast = @Cast, 
                NumberOfSeasons = @NumberOfSeasons, NumberOfEpisodes = @NumberOfEpisodes, 
                Language = @Language, Country = @Country, Rating = @Rating, 
                PosterUrl = @PosterUrl, LastUpdated = @LastUpdated 
                WHERE Id = @Id";
        var rowsAffected = db.Execute(sql, show);
        return rowsAffected > 0;
    }

    public bool DeleteShow(int id)
    {
        using var db = CreateConnection();
        var sql = "DELETE FROM Shows WHERE Id = @Id";
        var rowsAffected = db.Execute(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public IEnumerable<Show> SearchShows(string query)
    {
        using var db = CreateConnection();
        var sql = "SELECT * FROM Shows WHERE Title LIKE @Query OR Type LIKE @Query";
        return db.Query<Show>(sql, new { Query = $"%{query}%" });
    }

    public IEnumerable<Subtitle> SearchSubtitles(string query)
    {
        using var db = CreateConnection();
        var sql = @"SELECT s.* FROM Subtitles s
                JOIN Shows sh ON s.ShowId = sh.Id
                WHERE sh.Title LIKE @Query OR s.Language LIKE @Query OR s.Format LIKE @Query";
        return db.Query<Subtitle>(sql, new { Query = $"%{query}%" });
    }

    public bool RateSubtitle(int id, int rating)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO SubtitleRatings (SubtitleId, Rating) VALUES (@Id, @Rating)";
        var rowsAffected = db.Execute(sql, new { Id = id, Rating = rating });
        return rowsAffected > 0;
    }

    public IEnumerable<Subtitle> GetTopRatedSubtitles(int limit)
    {
        using var db = CreateConnection();
        var sql = @"SELECT s.*, AVG(sr.Rating) as AverageRating
                FROM Subtitles s
                LEFT JOIN SubtitleRatings sr ON s.Id = sr.SubtitleId
                GROUP BY s.Id
                ORDER BY AverageRating DESC
                LIMIT @Limit";
        return db.Query<Subtitle>(sql, new { Limit = limit });
    }
}