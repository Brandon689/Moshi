using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class SubscriptionRepository
{
    private readonly string _connectionString;

    public SubscriptionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Subscription>(
            "SELECT * FROM Subscriptions WHERE UserId = @UserId",
            new { UserId = userId });
    }

    public async Task<int> CreateAsync(Subscription subscription)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Subscriptions (UserId, ForumId, CreatedAt) 
                    VALUES (@UserId, @ForumId, @CreatedAt);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, subscription);
    }

    public async Task<bool> DeleteAsync(int userId, int forumId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Subscriptions WHERE UserId = @UserId AND ForumId = @ForumId";
        var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, ForumId = forumId });
        return affectedRows > 0;
    }

    public async Task<bool> ExistsAsync(int userId, int forumId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "SELECT COUNT(*) FROM Subscriptions WHERE UserId = @UserId AND ForumId = @ForumId";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, ForumId = forumId });
        return count > 0;
    }
}
