using Microsoft.Data.Sqlite;
namespace Moshi.Forums.Data;
public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Forums (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                ParentForumId INTEGER,
                CreatedAt DATETIME NOT NULL,
                UpdatedAt DATETIME NOT NULL,
                ThreadCount INTEGER NOT NULL DEFAULT 0,
                PostCount INTEGER NOT NULL DEFAULT 0,
                LastPostId INTEGER,
                LastPostAt DATETIME,
                DisplayOrder INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ParentForumId) REFERENCES Forums(Id)
            );

            CREATE TABLE IF NOT EXISTS Threads (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                ForumId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL,
                UpdatedAt DATETIME NOT NULL,
                ViewCount INTEGER NOT NULL DEFAULT 0,
                ReplyCount INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ForumId) REFERENCES Forums(Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );

            CREATE TABLE IF NOT EXISTS Posts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ThreadId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Content TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                UpdatedAt DATETIME NOT NULL,
                FOREIGN KEY (ThreadId) REFERENCES Threads(Id),
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );

            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                LastLoginAt DATETIME
            );
        ";

        await command.ExecuteNonQueryAsync();
    }
}
