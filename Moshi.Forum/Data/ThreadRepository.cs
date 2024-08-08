using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class ThreadRepository
{
    private readonly string _connectionString;

    public ThreadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<ForumThread>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<ForumThread>("SELECT * FROM Threads ORDER BY UpdatedAt DESC");
    }

    public async Task<ForumThread> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<ForumThread>("SELECT * FROM Threads WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(ForumThread thread)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Threads (Title, ForumId, UserId, CreatedAt, UpdatedAt, ViewCount, ReplyCount) 
                    VALUES (@Title, @ForumId, @UserId, @CreatedAt, @UpdatedAt, @ViewCount, @ReplyCount);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, thread);
    }

    public async Task<bool> UpdateAsync(ForumThread thread)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Threads 
                    SET Title = @Title, 
                        ForumId = @ForumId, 
                        UserId = @UserId, 
                        UpdatedAt = @UpdatedAt, 
                        ViewCount = @ViewCount, 
                        ReplyCount = @ReplyCount
                    WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, thread);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Threads WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task IncrementViewCountAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET ViewCount = ViewCount + 1 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task IncrementReplyCountAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET ReplyCount = ReplyCount + 1 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task UpdateLastPostAsync(int threadId, int postId, DateTime postCreatedAt)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET LastPostId = @PostId, LastPostAt = @PostCreatedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id",
            new { Id = threadId, PostId = postId, PostCreatedAt = postCreatedAt, UpdatedAt = DateTime.UtcNow });
    }

    public async Task LockThreadAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET IsLocked = 1 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task UnlockThreadAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET IsLocked = 0 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task PinThreadAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET IsPinned = 1 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task UnpinThreadAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Threads SET IsPinned = 0 WHERE Id = @Id",
            new { Id = threadId });
    }

    public async Task<IEnumerable<ThreadSearchResult>> SearchAsync(string query)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"
            SELECT t.*, u.Username, f.Name as ForumName
            FROM Threads t
            JOIN Users u ON t.UserId = u.Id
            JOIN Forums f ON t.ForumId = f.Id
            WHERE t.Title LIKE @Query
            ORDER BY t.CreatedAt DESC
            LIMIT 50";

        var results = await connection.QueryAsync<ThreadSearchResult>(sql, new { Query = $"%{query}%" });
        return results;
    }

    public async Task<IEnumerable<ForumThread>> GetThreadsByForumIdAsync(int forumId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<ForumThread>(
            "SELECT * FROM Threads WHERE ForumId = @ForumId ORDER BY UpdatedAt DESC",
            new { ForumId = forumId });
    }

    public async Task<int> GetThreadCountByForumIdAsync(int forumId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Threads WHERE ForumId = @ForumId",
            new { ForumId = forumId });
    }
}
