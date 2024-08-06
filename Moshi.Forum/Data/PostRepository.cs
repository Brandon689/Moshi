using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class PostRepository
{
    private readonly string _connectionString;

    public PostRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Post>("SELECT * FROM Posts");
    }

    public async Task<Post> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Post>("SELECT * FROM Posts WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(Post post)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Posts (ThreadId, UserId, Content, CreatedAt, UpdatedAt) 
                    VALUES (@ThreadId, @UserId, @Content, @CreatedAt, @UpdatedAt);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, post);
    }

    public async Task<bool> UpdateAsync(Post post)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Posts 
                    SET ThreadId = @ThreadId, 
                        UserId = @UserId, 
                        Content = @Content, 
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, post);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Posts WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<IEnumerable<Post>> GetPostsByThreadIdAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Post>("SELECT * FROM Posts WHERE ThreadId = @ThreadId ORDER BY CreatedAt", new { ThreadId = threadId });
    }

    public async Task<int> GetPostCountByThreadIdAsync(int threadId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Posts WHERE ThreadId = @ThreadId", new { ThreadId = threadId });
    }
}
