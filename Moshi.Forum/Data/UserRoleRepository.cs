using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class UserRoleRepository
{
    private readonly string _connectionString;

    public UserRoleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<UserRole>(
            "SELECT * FROM UserRoles WHERE UserId = @UserId",
            new { UserId = userId });
    }

    public async Task<int> CreateAsync(UserRole userRole)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO UserRoles (UserId, Role, AssignedAt) 
                    VALUES (@UserId, @Role, @AssignedAt);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, userRole);
    }

    public async Task<bool> DeleteAsync(int userRoleId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM UserRoles WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = userRoleId });
        return affectedRows > 0;
    }

    public async Task<bool> HasRoleAsync(int userId, Role role)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "SELECT COUNT(*) FROM UserRoles WHERE UserId = @UserId AND Role = @Role";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, Role = role });
        return count > 0;
    }

    public async Task BanUserAsync(int userId, DateTime banExpiresAt)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Users 
                SET IsBanned = 1, BanExpiresAt = @BanExpiresAt 
                WHERE Id = @UserId";
        await connection.ExecuteAsync(sql, new { UserId = userId, BanExpiresAt = banExpiresAt });
    }

    public async Task UnbanUserAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"UPDATE Users 
                SET IsBanned = 0, BanExpiresAt = NULL 
                WHERE Id = @UserId";
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

}
