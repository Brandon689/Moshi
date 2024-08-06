using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<User>("SELECT * FROM Users ORDER BY Username");
    }

    public async Task<User> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Users (Username, Email, PasswordHash, CreatedAt, LastLoginAt) 
                    VALUES (@Username, @Email, @PasswordHash, @CreatedAt, @LastLoginAt);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Users 
                    SET Username = @Username, 
                        Email = @Email, 
                        PasswordHash = @PasswordHash, 
                        LastLoginAt = @LastLoginAt
                    WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, user);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Users WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email });
    }

    public async Task UpdateLastLoginAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE Users SET LastLoginAt = @LastLoginAt WHERE Id = @Id",
            new { Id = id, LastLoginAt = DateTime.UtcNow });
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        var count = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Users WHERE Username = @Username",
            new { Username = username });
        return count > 0;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        using var connection = new SqliteConnection(_connectionString);
        var count = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Users WHERE Email = @Email",
            new { Email = email });
        return count > 0;
    }
}
