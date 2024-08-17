using Dapper;
using Microsoft.Data.Sqlite;

namespace Moshi.SubtitlesSite.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new ArgumentException("Connection string 'DefaultConnection' is not set in the configuration.");
        }
    }

    public void Initialize()
    {
        CreateDatabaseIfNotExists();
        CreateTablesIfNotExist();

        var sampleDataInserter = new SampleDataInserter(_connectionString);
        sampleDataInserter.InsertSampleData();
    }

    private void CreateDatabaseIfNotExists()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);
        var databasePath = connectionStringBuilder.DataSource;

        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new InvalidOperationException("Database path is not specified in the connection string.");
        }

        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(databasePath))
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
        }
    }

    private void CreateTablesIfNotExist()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Users table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                RegistrationDate TEXT NOT NULL,
                LastLoginDate TEXT,
                Country TEXT,
                IsAdmin BOOLEAN DEFAULT 0
            )");

        // Movies table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Movies (
                MovieId INTEGER PRIMARY KEY AUTOINCREMENT,
                ImdbId TEXT UNIQUE,
                Title TEXT NOT NULL,
                OriginalTitle TEXT,
                Year INTEGER NOT NULL,
                Synopsis TEXT,
                Genre TEXT,
                Director TEXT,
                Writers TEXT,
                Cast TEXT,
                Duration INTEGER,
                Language TEXT,
                Country TEXT,
                ImdbRating REAL,
                PosterUrl TEXT,
                DateAdded TEXT NOT NULL,
                LastUpdated TEXT
            )");

        // AlternativeTitles table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS AlternativeTitles (
                AlternativeTitleId INTEGER PRIMARY KEY AUTOINCREMENT,
                MovieId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                FOREIGN KEY (MovieId) REFERENCES Movies(MovieId)
            )");

        // Subtitles table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Subtitles (
                SubtitleId INTEGER PRIMARY KEY AUTOINCREMENT,
                MovieId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Language TEXT NOT NULL,
                Format TEXT NOT NULL,
                ReleaseInfo TEXT,
                StorageFileName TEXT NOT NULL,
                OriginalFileName TEXT NOT NULL,
                UploadDate TEXT NOT NULL,
                Downloads INTEGER NOT NULL DEFAULT 0,
                FPS REAL,
                NumDiscs INTEGER DEFAULT 1,
                Notes TEXT,
                FOREIGN KEY (MovieId) REFERENCES Movies(MovieId),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            )");

        // SubtitleRatings table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS SubtitleRatings (
                RatingId INTEGER PRIMARY KEY AUTOINCREMENT,
                SubtitleId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Rating INTEGER NOT NULL CHECK (Rating BETWEEN 1 AND 10),
                RatingDate TEXT NOT NULL,
                FOREIGN KEY (SubtitleId) REFERENCES Subtitles(SubtitleId),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            )");

        // SubtitleComments table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS SubtitleComments (
                CommentId INTEGER PRIMARY KEY AUTOINCREMENT,
                SubtitleId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                Comment TEXT NOT NULL,
                CommentDate TEXT NOT NULL,
                FOREIGN KEY (SubtitleId) REFERENCES Subtitles(SubtitleId),
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS ProfileComments (
            CommentId INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            CommenterId INTEGER NOT NULL,
            CommentText TEXT NOT NULL,
            CommentDate TEXT NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(UserId),
            FOREIGN KEY (CommenterId) REFERENCES Users(UserId)
        );");

        // UserBadges table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS UserBadges (
                BadgeId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                BadgeName TEXT NOT NULL,
                AwardDate TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(UserId)
            )");

        // MovieLinks table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS MovieLinks (
                LinkId INTEGER PRIMARY KEY AUTOINCREMENT,
                MovieId INTEGER NOT NULL,
                LinkType TEXT NOT NULL,
                Url TEXT NOT NULL,
                FOREIGN KEY (MovieId) REFERENCES Movies(MovieId)
            )");
    }
}