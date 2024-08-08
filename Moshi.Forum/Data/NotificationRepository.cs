using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Forums.Models;

namespace Moshi.Forums.Data;

public class NotificationRepository
{
    private readonly string _connectionString;

    public NotificationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Notification>(
            "SELECT * FROM Notifications WHERE UserId = @UserId ORDER BY CreatedAt DESC",
            new { UserId = userId });
    }

    public async Task<int> CreateAsync(Notification notification)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = @"INSERT INTO Notifications (UserId, Message, Type, CreatedAt, IsRead) 
                    VALUES (@UserId, @Message, @Type, @CreatedAt, @IsRead);
                    SELECT last_insert_rowid();";
        return await connection.ExecuteScalarAsync<int>(sql, notification);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "UPDATE Notifications SET IsRead = 1 WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = notificationId });
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int notificationId)
    {
        using var connection = new SqliteConnection(_connectionString);
        var sql = "DELETE FROM Notifications WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = notificationId });
        return affectedRows > 0;
    }
}
