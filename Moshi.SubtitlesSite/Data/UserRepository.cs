using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.SubtitlesSite.Models;
using System.Data;

namespace Moshi.SubtitlesSite.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public User GetUserById(int userId)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE UserId = @UserId", new { UserId = userId });
    }

    public User GetUserByUsername(string username)
    {
        using var db = CreateConnection();
        return db.QueryFirstOrDefault<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
    }

    public bool UpdateUser(User user)
    {
        using var db = CreateConnection();
        var sql = @"UPDATE Users 
                    SET Username = @Username, Email = @Email, LastLoginDate = @LastLoginDate, Country = @Country 
                    WHERE UserId = @UserId";
        var rowsAffected = db.Execute(sql, user);
        return rowsAffected > 0;
    }

    public IEnumerable<UserBadge> GetUserBadges(int userId)
    {
        using var db = CreateConnection();
        return db.Query<UserBadge>(@"
            SELECT * FROM UserBadges 
            WHERE UserId = @UserId 
            ORDER BY AwardDate DESC", new { UserId = userId });
    }

    public bool AddUserBadge(UserBadge badge)
    {
        using var db = CreateConnection();
        var sql = "INSERT INTO UserBadges (UserId, BadgeName, AwardDate) VALUES (@UserId, @BadgeName, @AwardDate)";
        var rowsAffected = db.Execute(sql, badge);
        return rowsAffected > 0;
    }

    public IEnumerable<ProfileComment> GetUserProfileComments(int userId)
    {
        using var db = CreateConnection();
        return db.Query<ProfileComment>(@"
            SELECT c.*, u.Username as CommenterUsername
            FROM ProfileComments c
            JOIN Users u ON c.CommenterId = u.UserId
            WHERE c.UserId = @UserId
            ORDER BY c.CommentDate DESC", new { UserId = userId });
    }

    public bool AddProfileComment(ProfileComment comment)
    {
        using var db = CreateConnection();
        var sql = @"INSERT INTO ProfileComments (UserId, CommenterId, CommentText, CommentDate) 
                    VALUES (@UserId, @CommenterId, @CommentText, @CommentDate)";
        var rowsAffected = db.Execute(sql, comment);
        return rowsAffected > 0;
    }

    public int GetUserUploadedSubtitlesCount(int userId)
    {
        using var db = CreateConnection();
        return db.ExecuteScalar<int>("SELECT COUNT(*) FROM Subtitles WHERE UserId = @UserId", new { UserId = userId });
    }
}