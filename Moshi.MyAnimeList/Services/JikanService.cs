using JikanDotNet;
using Microsoft.Data.Sqlite;
using Dapper;
using Moshi.MyAnimeList.JikanClients;
using Moshi.MyAnimeList.Models;

namespace Moshi.MyAnimeList.Services
{
    public class JikanService
    {
        private readonly JikanClient _client;
        private readonly string _connectionString;

        public JikanService(string connectionString)
        {
            _client = new JikanClient();
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS JikanAnime (
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
                )");
        }

        public async Task<JikanAnime> GetAnime(int id)
        {
            var animeFromDb = await GetAnimeFromDatabase(id);
            if (animeFromDb != null)
            {
                return animeFromDb;
            }

            var animeFromApi = await _client.GetAnime(id);
            var jikanAnime = DtoMapper.ToDto(animeFromApi);

            await SaveAnimeToDatabase(jikanAnime);

            return jikanAnime;
        }

        private async Task<JikanAnime> GetAnimeFromDatabase(int malId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM JikanAnime WHERE MalId = @MalId";
            return await connection.QueryFirstOrDefaultAsync<JikanAnime>(sql, new { MalId = malId });
        }

        private async Task SaveAnimeToDatabase(JikanAnime anime)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                INSERT INTO JikanAnime (MalId, Title, TitleEnglish, TitleJapanese, Type, Episodes, 
                                   Duration, Rating, Score, ScoredBy, Synopsis, Background, Year)
                VALUES (@MalId, @Title, @TitleEnglish, @TitleJapanese, @Type, @Episodes, 
                        @Duration, @Rating, @Score, @ScoredBy, @Synopsis, @Background, @Year)";

            await connection.ExecuteAsync(sql, anime);
        }
    }
}
