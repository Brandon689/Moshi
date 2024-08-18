using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System.Data;

namespace Moshi.SubtitlesSite.Data;

public class SubtitleRequestRepository
{
    private readonly string _connectionString;

    public SubtitleRequestRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public IEnumerable<SubtitleRequest> GetSubtitleRequests(int count)
    {
        using var db = CreateConnection();
        return db.Query<SubtitleRequest>(@"
        SELECT * FROM SubtitleRequests 
        ORDER BY LatestUploadDate DESC 
        LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<SubtitleRequest> GetAllSubtitleRequests()
    {
        using var db = CreateConnection();
        return db.Query<SubtitleRequest>("SELECT * FROM SubtitleRequests ORDER BY LatestUploadDate DESC");
    }

    public SubtitleRequest GetSubtitleRequestById(int id)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<SubtitleRequest>("SELECT * FROM SubtitleRequests WHERE Id = @Id", new { Id = id });
    }

    public int CreateSubtitleRequest(SubtitleRequest request)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO SubtitleRequests (MovieName, Year, Rating, LatestUploadDate, LatestSubtitleLanguage, SubtitleCount) 
                    VALUES (@MovieName, @Year, @Rating, @LatestUploadDate, @LatestSubtitleLanguage, @SubtitleCount);
                    SELECT last_insert_rowid()";
        return db.ExecuteScalar<int>(sql, request);
    }

    public bool UpdateSubtitleRequest(SubtitleRequest request)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE SubtitleRequests 
                    SET MovieName = @MovieName, Year = @Year, Rating = @Rating, 
                        LatestUploadDate = @LatestUploadDate, LatestSubtitleLanguage = @LatestSubtitleLanguage, 
                        SubtitleCount = @SubtitleCount 
                    WHERE Id = @Id";
        var rowsAffected = db.Execute(sql, request);
        return rowsAffected > 0;
    }

    public bool DeleteSubtitleRequest(int id)
    {
        using var db = CreateConnection();
        var sql = "DELETE FROM SubtitleRequests WHERE Id = @Id";
        var rowsAffected = db.Execute(sql, new { Id = id });
        return rowsAffected > 0;
    }

    public IEnumerable<SubtitleRequest> SearchSubtitleRequests(string query)
    {
        using var db = CreateConnection();
        var sql = "SELECT * FROM SubtitleRequests WHERE MovieName LIKE @Query OR Year LIKE @Query";
        return db.Query<SubtitleRequest>(sql, new { Query = $"%{query}%" });
    }

    public IEnumerable<SubtitleRequest> GetMostRecentSubtitleRequests(int count)
    {
        using var db = CreateConnection();
        return db.Query<SubtitleRequest>(@"
            SELECT * FROM SubtitleRequests 
            ORDER BY LatestUploadDate DESC 
            LIMIT @Count",
            new { Count = count });
    }

    public IEnumerable<SubtitleRequest> GetSubtitleRequestsByLanguage(string language)
    {
        using var db = CreateConnection();
        return db.Query<SubtitleRequest>(
            "SELECT * FROM SubtitleRequests WHERE LatestSubtitleLanguage = @Language",
            new { Language = language });
    }

    public int GetTotalSubtitleRequestCount()
    {
        using var db = CreateConnection();
        return db.ExecuteScalar<int>("SELECT COUNT(*) FROM SubtitleRequests");
    }
}
