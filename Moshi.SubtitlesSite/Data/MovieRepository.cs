using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System.Data;

namespace Moshi.SubtitlesSite.Data;

public class MovieRepository
{
    private readonly string _connectionString;

    public MovieRepository(IConfiguration configuration)
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
}
