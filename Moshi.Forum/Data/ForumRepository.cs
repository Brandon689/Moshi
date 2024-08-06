using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class ForumRepository
{
    private readonly string _connectionString;

    public ForumRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Forum>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Forum>("SELECT * FROM Forums ORDER BY DisplayOrder");
    }

    public async Task<Forum> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Forum>("SELECT * FROM Forums WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(Forum forum)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Forums (Name, Description, ParentForumId, CreatedAt, UpdatedAt, ThreadCount, PostCount, LastPostId, LastPostAt, DisplayOrder) 
                VALUES (@Name, @Description, @ParentForumId, @CreatedAt, @UpdatedAt, @ThreadCount, @PostCount, @LastPostId, @LastPostAt, @DisplayOrder);
                SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, forum);
    }

    public async Task<bool> UpdateAsync(Forum forum)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Forums 
                    SET Name = @Name, 
                        Description = @Description, 
                        ParentForumId = @ParentForumId, 
                        UpdatedAt = @UpdatedAt, 
                        ThreadCount = @ThreadCount, 
                        PostCount = @PostCount, 
                        LastPostId = @LastPostId, 
                        LastPostAt = @LastPostAt, 
                        DisplayOrder = @DisplayOrder
                    WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, forum);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Forums WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
}
