using Dapper;
using Microsoft.Data.Sqlite;

namespace Moshi.Blog.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    IsAdmin INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT
                );

                CREATE TABLE IF NOT EXISTS Posts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT,
                    AuthorId INTEGER NOT NULL,
                    CategoryId INTEGER NOT NULL,
                    FOREIGN KEY (AuthorId) REFERENCES Users (Id),
                    FOREIGN KEY (CategoryId) REFERENCES Categories (Id)
                );

                CREATE TABLE IF NOT EXISTS Comments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Content TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    PostId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    FOREIGN KEY (PostId) REFERENCES Posts (Id),
                    FOREIGN KEY (UserId) REFERENCES Users (Id)
                );
            ");
        }
    }
}