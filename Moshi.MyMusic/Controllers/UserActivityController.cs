using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public UserActivityController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // POST: api/UserActivity/RecordListen
        [HttpPost("RecordListen")]
        public async Task<IActionResult> RecordListen(ListeningHistory history)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO listening_history (user_id, song_id, listened_at, duration_listened)
                        VALUES (@UserId, @SongId, @ListenedAt, @DurationListened);
                        SELECT last_insert_rowid();";

            history.ListenedAt = DateTime.UtcNow;

            var id = await connection.ExecuteScalarAsync<int>(sql, history);
            history.HistoryId = id;

            return CreatedAtAction(nameof(GetListeningHistory), new { userId = history.UserId }, history);
        }

        // GET: api/UserActivity/ListeningHistory/5
        [HttpGet("ListeningHistory/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetListeningHistory(int userId, int limit = 50)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT lh.*, s.title AS song_title, a.name AS artist_name
                        FROM listening_history lh
                        JOIN songs s ON lh.song_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE lh.user_id = @UserId
                        ORDER BY lh.listened_at DESC
                        LIMIT @Limit";

            var history = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(history);
        }

        // GET: api/UserActivity/TopSongs/5
        [HttpGet("TopSongs/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetTopSongs(int userId, int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT s.*, a.name AS artist_name, COUNT(*) AS listen_count
                        FROM listening_history lh
                        JOIN songs s ON lh.song_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE lh.user_id = @UserId
                        GROUP BY s.song_id
                        ORDER BY listen_count DESC
                        LIMIT @Limit";

            var topSongs = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(topSongs);
        }

        // GET: api/UserActivity/TopArtists/5
        [HttpGet("TopArtists/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetTopArtists(int userId, int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT a.*, COUNT(*) AS listen_count
                        FROM listening_history lh
                        JOIN songs s ON lh.song_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE lh.user_id = @UserId
                        GROUP BY a.artist_id
                        ORDER BY listen_count DESC
                        LIMIT @Limit";

            var topArtists = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(topArtists);
        }

        // GET: api/UserActivity/RecentlyPlayed/5
        [HttpGet("RecentlyPlayed/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetRecentlyPlayed(int userId, int limit = 20)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT DISTINCT s.*, a.name AS artist_name, MAX(lh.listened_at) AS last_played
                        FROM listening_history lh
                        JOIN songs s ON lh.song_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE lh.user_id = @UserId
                        GROUP BY s.song_id
                        ORDER BY last_played DESC
                        LIMIT @Limit";

            var recentlyPlayed = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(recentlyPlayed);
        }

        // GET: api/UserActivity/Recommendations/5
        [HttpGet("Recommendations/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetRecommendations(int userId, int limit = 20)
        {
            using var connection = CreateConnection();
            // This is a simplified recommendation algorithm.
            // In a real-world scenario, you'd want to use more sophisticated methods.
            var sql = @"SELECT s.*, a.name AS artist_name, COUNT(*) AS relevance_score
                        FROM listening_history lh
                        JOIN songs s ON lh.song_id = s.song_id
                        JOIN song_artists sa ON s.song_id = sa.song_id
                        JOIN artists a ON sa.artist_id = a.artist_id
                        WHERE a.artist_id IN (
                            SELECT DISTINCT sa2.artist_id
                            FROM listening_history lh2
                            JOIN songs s2 ON lh2.song_id = s2.song_id
                            JOIN song_artists sa2 ON s2.song_id = sa2.song_id
                            WHERE lh2.user_id = @UserId
                        )
                        AND s.song_id NOT IN (
                            SELECT song_id
                            FROM listening_history
                            WHERE user_id = @UserId
                        )
                        GROUP BY s.song_id
                        ORDER BY relevance_score DESC
                        LIMIT @Limit";

            var recommendations = await connection.QueryAsync(sql, new { UserId = userId, Limit = limit });
            return Ok(recommendations);
        }

        // POST: api/UserActivity/SaveRecommendation
        [HttpPost("SaveRecommendation")]
        public async Task<IActionResult> SaveRecommendation(UserRecommendation recommendation)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO user_recommendations (user_id, item_type, item_id, score, generated_at)
                        VALUES (@UserId, @ItemType, @ItemId, @Score, @GeneratedAt)";

            recommendation.GeneratedAt = DateTime.UtcNow;

            await connection.ExecuteAsync(sql, recommendation);

            return NoContent();
        }
    }

    public class UserRecommendation
    {
        public int UserId { get; set; }
        public string ItemType { get; set; }
        public int ItemId { get; set; }
        public float Score { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}