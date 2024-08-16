using Dapper;
using Microsoft.Data.Sqlite;

namespace Moshi.MyAnimeList.Data;

public class JikanDatabaseInitializer
{
    private readonly string _connectionString;

    public JikanDatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateTables()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var sql = @"
            CREATE TABLE Anime (
                AnimeId INTEGER PRIMARY KEY AUTOINCREMENT,
                MalId INTEGER,
                Title TEXT,
                TitleEnglish TEXT,
                TitleJapanese TEXT,
                Type TEXT,
                Episodes INTEGER,
                Duration TEXT,
                Rating TEXT,
                Score REAL,
                ScoredBy INTEGER,
                Synopsis TEXT,
                Background TEXT,
                Year INTEGER
            );";
            connection.Execute(sql);
        }
    }
}