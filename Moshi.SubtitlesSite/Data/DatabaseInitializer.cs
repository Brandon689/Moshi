using Microsoft.Data.Sqlite;

namespace SubtitlesSite.Data;

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
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                // The database file will be created by opening the connection
            }
        }
    }

    private void CreateTablesIfNotExist()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create Shows table with new columns
        var createShowsTableCommand = connection.CreateCommand();
        createShowsTableCommand.CommandText = @"
        CREATE TABLE IF NOT EXISTS Shows (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Year INTEGER NOT NULL,
            Type TEXT NOT NULL,
            Description TEXT,
            Genre TEXT,
            Director TEXT,
            Cast TEXT,
            NumberOfSeasons INTEGER,
            NumberOfEpisodes INTEGER,
            Language TEXT,
            Country TEXT,
            Rating REAL,
            PosterUrl TEXT,
            DateAdded TEXT NOT NULL,
            LastUpdated TEXT
        )";
        createShowsTableCommand.ExecuteNonQuery();

        // Create Subtitles table
        var createSubtitlesTableCommand = connection.CreateCommand();
        createSubtitlesTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Subtitles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ShowId INTEGER NOT NULL,
                Language TEXT NOT NULL,
                Format TEXT NOT NULL,
                StorageFileName TEXT NOT NULL,
                OriginalFileName TEXT NOT NULL,
                UploadDate TEXT NOT NULL,
                Downloads INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (ShowId) REFERENCES Shows(Id)
            )";
        createSubtitlesTableCommand.ExecuteNonQuery();

        // Create SubtitleRatings table
        var createSubtitleRatingsTableCommand = connection.CreateCommand();
        createSubtitleRatingsTableCommand.CommandText = @"
        CREATE TABLE IF NOT EXISTS SubtitleRatings (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SubtitleId INTEGER NOT NULL,
            Rating INTEGER NOT NULL,
            FOREIGN KEY (SubtitleId) REFERENCES Subtitles(Id)
        )";
        createSubtitleRatingsTableCommand.ExecuteNonQuery();
    }
}