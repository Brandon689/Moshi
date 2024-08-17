using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System.Data;

namespace Moshi.SubtitlesSite.Data;

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

    public IEnumerable<Movie> GetAllMovies()
    {
        using var db = CreateConnection();
        var sql = @"
        SELECT m.*, 
               CAST(COUNT(sub.SubtitleId) AS INTEGER) as SubtitleCount 
        FROM Movies m 
        LEFT JOIN Subtitles sub ON m.MovieId = sub.MovieId 
        GROUP BY m.MovieId";

        return db.Query<Movie>(sql);
    }

    public Movie GetMovieById(int id)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<Movie>("SELECT * FROM Movies WHERE MovieId = @Id", new { Id = id });
    }

    public int CreateMovie(Movie movie)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO Movies (ImdbId, Title, OriginalTitle, Year, Synopsis, Genre, Director, Writers, Cast, 
                Duration, Language, Country, ImdbRating, PosterUrl, DateAdded, LastUpdated) 
                VALUES (@ImdbId, @Title, @OriginalTitle, @Year, @Synopsis, @Genre, @Director, @Writers, @Cast, 
                @Duration, @Language, @Country, @ImdbRating, @PosterUrl, @DateAdded, @LastUpdated);
                SELECT last_insert_rowid()";
        return db.ExecuteScalar<int>(sql, movie);
    }

    public IEnumerable<Subtitle> GetSubtitlesByMovieId(int movieId)
    {
        using var db = CreateConnection();
        return db.Query<Subtitle>("SELECT * FROM Subtitles WHERE MovieId = @MovieId", new { MovieId = movieId });
    }

    public Subtitle GetSubtitleById(int id)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<Subtitle>("SELECT * FROM Subtitles WHERE SubtitleId = @Id", new { Id = id });
    }

    public int CreateSubtitle(Subtitle subtitle)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO Subtitles (MovieId, UserId, Language, Format, ReleaseInfo, StorageFileName, OriginalFileName, UploadDate, Downloads, FPS, NumDiscs, Notes) 
                    VALUES (@MovieId, @UserId, @Language, @Format, @ReleaseInfo, @StorageFileName, @OriginalFileName, @UploadDate, @Downloads, @FPS, @NumDiscs, @Notes);
                    SELECT last_insert_rowid()";
        return db.ExecuteScalar<int>(sql, subtitle);
    }

    public void IncrementDownloadCount(int subtitleId)
    {
        using var db = CreateConnection();
        db.Execute("UPDATE Subtitles SET Downloads = Downloads + 1 WHERE SubtitleId = @Id", new { Id = subtitleId });
    }

    public bool UpdateSubtitle(Subtitle subtitle)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE Subtitles 
                    SET MovieId = @MovieId, UserId = @UserId, Language = @Language, Format = @Format, 
                        ReleaseInfo = @ReleaseInfo, StorageFileName = @StorageFileName, 
                        OriginalFileName = @OriginalFileName, UploadDate = @UploadDate, 
                        Downloads = @Downloads, FPS = @FPS, NumDiscs = @NumDiscs, Notes = @Notes
                    WHERE SubtitleId = @SubtitleId";
        var rowsAffected = db.Execute(sql, subtitle);
        return rowsAffected > 0;
    }

    public bool DeleteSubtitle(int id)
    {
        using var db = CreateConnection();
        var rowsAffected = db.Execute("DELETE FROM Subtitles WHERE SubtitleId = @Id", new { Id = id });
        return rowsAffected > 0;
    }

    public bool UpdateMovie(Movie movie)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE Movies 
                SET ImdbId = @ImdbId, Title = @Title, OriginalTitle = @OriginalTitle, 
                Year = @Year, Synopsis = @Synopsis, Genre = @Genre, Director = @Director, 
                Writers = @Writers, Cast = @Cast, Duration = @Duration, Language = @Language, 
                Country = @Country, ImdbRating = @ImdbRating, PosterUrl = @PosterUrl, 
                LastUpdated = @LastUpdated 
                WHERE MovieId = @MovieId";
        var rowsAffected = db.Execute(sql, movie);
        return rowsAffected > 0;
    }

    public bool DeleteMovie(int id)
    {
        using var db = CreateConnection();
        var sql = "DELETE FROM Movies WHERE MovieId = @Id";
        var rowsAffected = db.Execute(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public IEnumerable<Movie> SearchMovies(string query)
    {
        using var db = CreateConnection();
        var sql = "SELECT * FROM Movies WHERE Title LIKE @Query OR OriginalTitle LIKE @Query OR Synopsis LIKE @Query";
        return db.Query<Movie>(sql, new { Query = $"%{query}%" });
    }

    public IEnumerable<Subtitle> SearchSubtitles(string query)
    {
        using var db = CreateConnection();
        var sql = @"SELECT s.* FROM Subtitles s
                JOIN Movies m ON s.MovieId = m.MovieId
                WHERE m.Title LIKE @Query OR s.Language LIKE @Query OR s.Format LIKE @Query OR s.ReleaseInfo LIKE @Query";
        return db.Query<Subtitle>(sql, new { Query = $"%{query}%" });
    }

    public bool RateSubtitle(int subtitleId, int userId, float rating)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO SubtitleRatings (SubtitleId, UserId, Rating, RatingDate) 
                    VALUES (@SubtitleId, @UserId, @Rating, @RatingDate)
                    ON CONFLICT(SubtitleId, UserId) 
                    DO UPDATE SET Rating = @Rating, RatingDate = @RatingDate";
        var rowsAffected = db.Execute(sql, new { SubtitleId = subtitleId, UserId = userId, Rating = rating, RatingDate = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public IEnumerable<Subtitle> GetTopRatedSubtitles(int limit)
    {
        using var db = CreateConnection();
        var sql = @"SELECT s.*, AVG(sr.Rating) as AverageRating
                FROM Subtitles s
                LEFT JOIN SubtitleRatings sr ON s.SubtitleId = sr.SubtitleId
                GROUP BY s.SubtitleId
                ORDER BY AverageRating DESC
                LIMIT @Limit";
        return db.Query<Subtitle>(sql, new { Limit = limit });
    }

    public IEnumerable<AlternativeTitle> GetAlternativeTitles(int movieId)
    {
        using var db = CreateConnection();
        return db.Query<AlternativeTitle>("SELECT * FROM AlternativeTitles WHERE MovieId = @MovieId", new { MovieId = movieId });
    }

    public bool AddAlternativeTitle(AlternativeTitle alternativeTitle)
    {
        using var db = CreateConnection();
        var sql = "INSERT INTO AlternativeTitles (MovieId, Title) VALUES (@MovieId, @Title)";
        var rowsAffected = db.Execute(sql, alternativeTitle);
        return rowsAffected > 0;
    }

    public IEnumerable<MovieLink> GetMovieLinks(int movieId)
    {
        using var db = CreateConnection();
        return db.Query<MovieLink>("SELECT * FROM MovieLinks WHERE MovieId = @MovieId", new { MovieId = movieId });
    }

    public bool AddMovieLink(MovieLink movieLink)
    {
        using var db = CreateConnection();
        var sql = "INSERT INTO MovieLinks (MovieId, LinkType, Url) VALUES (@MovieId, @LinkType, @Url)";
        var rowsAffected = db.Execute(sql, movieLink);
        return rowsAffected > 0;
    }

    public IEnumerable<SubtitleComment> GetSubtitleComments(int subtitleId)
    {
        using var db = CreateConnection();
        return db.Query<SubtitleComment>("SELECT * FROM SubtitleComments WHERE SubtitleId = @SubtitleId ORDER BY CommentDate DESC", new { SubtitleId = subtitleId });
    }
    public IEnumerable<SubtitleCommentWithUsername> GetCommentsWithUsernames(int movieId)
    {
        using var db = CreateConnection();
        var sql = @"
        SELECT sc.Comment, sc.CommentDate, u.Username
        FROM SubtitleComments sc
        JOIN Users u ON sc.UserId = u.UserId
        JOIN Subtitles s ON sc.SubtitleId = s.SubtitleId
        WHERE s.MovieId = @MovieId
        ORDER BY sc.CommentDate DESC";

        return db.Query<SubtitleCommentWithUsername>(sql, new { MovieId = movieId });
    }
    public bool AddSubtitleComment(SubtitleComment comment)
    {
        using var db = CreateConnection();
        var sql = "INSERT INTO SubtitleComments (SubtitleId, UserId, Comment, CommentDate) VALUES (@SubtitleId, @UserId, @Comment, @CommentDate)";
        var rowsAffected = db.Execute(sql, comment);
        return rowsAffected > 0;
    }
    public IEnumerable<SubtitleWithMovieDetails> GetSubtitlesWithMovieDetails(int count, string orderBy = "UploadDate")
    {
        using var db = CreateConnection();
        string orderByClause;

        switch (orderBy.ToLower())
        {
            case "imdbrating":
                orderByClause = "m.ImdbRating";
                break;
            case "downloads":
                orderByClause = "s.Downloads";
                break;
            case "uploaddate":
            default:
                orderByClause = "s.UploadDate";
                break;
        }

        var sql = $@"
        SELECT s.SubtitleId, m.Title AS MovieTitle, u.Username, s.Downloads, m.ImdbRating
        FROM Subtitles s
        JOIN Movies m ON s.MovieId = m.MovieId
        JOIN Users u ON s.UserId = u.UserId
        ORDER BY {orderByClause} DESC
        LIMIT @Count";

        return db.Query<SubtitleWithMovieDetails>(sql, new { Count = count });
    }
    public int GetSubtitleCommentCount(int subtitleId)
    {
        using var db = CreateConnection();
        return db.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SubtitleComments WHERE SubtitleId = @SubtitleId",
            new { SubtitleId = subtitleId }
        );
    }

    public double GetAverageRating(int subtitleId)
    {
        using var db = CreateConnection();
        return db.ExecuteScalar<double>("SELECT AVG(Rating) FROM SubtitleRatings WHERE SubtitleId = @SubtitleId", new { SubtitleId = subtitleId });
    }
    public IEnumerable<Subtitle> GetLatestSubtitles(int count)
    {
        using var db = CreateConnection();
        return db.Query<Subtitle>(@"
            SELECT * FROM Subtitles
            ORDER BY UploadDate DESC
            LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<Subtitle> GetMostDownloadedSubtitles(int count)
    {
        using var db = CreateConnection();
        return db.Query<Subtitle>(@"
            SELECT * FROM Subtitles
            ORDER BY Downloads DESC
            LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<(string Username, int UploadCount, DateTime LatestUpload)> GetTopUploaders(int count)
    {
        using var db = CreateConnection();
        return db.Query<(string, int, DateTime)>(@"
            SELECT u.Username, COUNT(s.SubtitleId) AS UploadCount, MAX(s.UploadDate) AS LatestUpload
            FROM Users u
            JOIN Subtitles s ON u.UserId = s.UserId
            GROUP BY u.UserId
            ORDER BY UploadCount DESC
            LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<Movie> GetMoviesWithMostSubtitles(int count)
    {
        using var db = CreateConnection();
        return db.Query<Movie>(@"
        SELECT m.*, COUNT(s.SubtitleId) AS SubtitleCount
        FROM Movies m
        JOIN Subtitles s ON m.MovieId = s.MovieId
        GROUP BY m.MovieId
        ORDER BY SubtitleCount DESC
        LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<SubtitleCommentWithUsername> GetLatestComments(int count)
    {
        using var db = CreateConnection();
        return db.Query<SubtitleCommentWithUsername>(@"
            SELECT sc.*, u.Username, m.Title AS MovieName
            FROM SubtitleComments sc
            JOIN Users u ON sc.UserId = u.UserId
            JOIN Subtitles s ON sc.SubtitleId = s.SubtitleId
            JOIN Movies m ON s.MovieId = m.MovieId
            ORDER BY sc.CommentDate DESC
            LIMIT @Count",
            new { Count = count });
    }
}