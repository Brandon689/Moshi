using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public ReportingController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Reporting/TopSongs
        [HttpGet("TopSongs")]
        public async Task<ActionResult<IEnumerable<TopSong>>> GetTopSongs(int limit = 10, int days = 30)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT s.song_id, s.title, a.name as artist_name, COUNT(*) as play_count
                FROM listening_history lh
                JOIN songs s ON lh.song_id = s.song_id
                JOIN song_artists sa ON s.song_id = sa.song_id
                JOIN artists a ON sa.artist_id = a.artist_id
                WHERE lh.listened_at >= @StartDate
                GROUP BY s.song_id
                ORDER BY play_count DESC
                LIMIT @Limit";

            var startDate = DateTime.UtcNow.AddDays(-days);
            var topSongs = await connection.QueryAsync<TopSong>(sql, new { StartDate = startDate, Limit = limit });

            return Ok(topSongs);
        }

        // GET: api/Reporting/TopArtists
        [HttpGet("TopArtists")]
        public async Task<ActionResult<IEnumerable<TopArtist>>> GetTopArtists(int limit = 10, int days = 30)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT a.artist_id, a.name, COUNT(DISTINCT lh.user_id) as listener_count
                FROM listening_history lh
                JOIN songs s ON lh.song_id = s.song_id
                JOIN song_artists sa ON s.song_id = sa.song_id
                JOIN artists a ON sa.artist_id = a.artist_id
                WHERE lh.listened_at >= @StartDate
                GROUP BY a.artist_id
                ORDER BY listener_count DESC
                LIMIT @Limit";

            var startDate = DateTime.UtcNow.AddDays(-days);
            var topArtists = await connection.QueryAsync<TopArtist>(sql, new { StartDate = startDate, Limit = limit });

            return Ok(topArtists);
        }

        // GET: api/Reporting/UserGrowth
        [HttpGet("UserGrowth")]
        [Authorize(Roles = "Admin")] // Assuming you have role-based authorization
        public async Task<ActionResult<IEnumerable<UserGrowthData>>> GetUserGrowth(int months = 12)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    strftime('%Y-%m', created_at) as month,
                    COUNT(*) as new_users
                FROM users
                WHERE created_at >= @StartDate
                GROUP BY strftime('%Y-%m', created_at)
                ORDER BY month";

            var startDate = DateTime.UtcNow.AddMonths(-months);
            var userGrowth = await connection.QueryAsync<UserGrowthData>(sql, new { StartDate = startDate });

            return Ok(userGrowth);
        }

        // GET: api/Reporting/GenrePopularity
        [HttpGet("GenrePopularity")]
        public async Task<ActionResult<IEnumerable<GenrePopularity>>> GetGenrePopularity()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT g.name as genre, COUNT(*) as listen_count
                FROM listening_history lh
                JOIN songs s ON lh.song_id = s.song_id
                JOIN song_genres sg ON s.song_id = sg.song_id
                JOIN genres g ON sg.genre_id = g.genre_id
                GROUP BY g.genre_id
                ORDER BY listen_count DESC";

            var genrePopularity = await connection.QueryAsync<GenrePopularity>(sql);

            return Ok(genrePopularity);
        }

        // GET: api/Reporting/UserActivity
        [HttpGet("UserActivity")]
        public async Task<ActionResult<UserActivity>> GetUserActivity()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM listening_history WHERE user_id = @UserId) as total_listens,
                    (SELECT COUNT(DISTINCT song_id) FROM listening_history WHERE user_id = @UserId) as unique_songs,
                    (SELECT COUNT(*) FROM playlists WHERE user_id = @UserId) as playlist_count,
                    (SELECT COUNT(*) FROM user_followers WHERE follower_id = @UserId) as following_count,
                    (SELECT COUNT(*) FROM user_followers WHERE followed_id = @UserId) as follower_count";

            var userActivity = await connection.QuerySingleOrDefaultAsync<UserActivity>(sql, new { UserId = userId });

            return Ok(userActivity);
        }

        // GET: api/Reporting/SystemStats
        [HttpGet("SystemStats")]
        [Authorize(Roles = "Admin")] // Assuming you have role-based authorization
        public async Task<ActionResult<SystemStats>> GetSystemStats()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM users) as total_users,
                    (SELECT COUNT(*) FROM songs) as total_songs,
                    (SELECT COUNT(*) FROM artists) as total_artists,
                    (SELECT COUNT(*) FROM albums) as total_albums,
                    (SELECT COUNT(*) FROM playlists) as total_playlists,
                    (SELECT COUNT(*) FROM podcasts) as total_podcasts";

            var systemStats = await connection.QuerySingleOrDefaultAsync<SystemStats>(sql);

            return Ok(systemStats);
        }
    }

    public class TopSong
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public int PlayCount { get; set; }
    }

    public class TopArtist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public int ListenerCount { get; set; }
    }

    public class UserGrowthData
    {
        public string Month { get; set; }
        public int NewUsers { get; set; }
    }

    public class GenrePopularity
    {
        public string Genre { get; set; }
        public int ListenCount { get; set; }
    }

    public class UserActivity
    {
        public int TotalListens { get; set; }
        public int UniqueSongs { get; set; }
        public int PlaylistCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
    }

    public class SystemStats
    {
        public int TotalUsers { get; set; }
        public int TotalSongs { get; set; }
        public int TotalArtists { get; set; }
        public int TotalAlbums { get; set; }
        public int TotalPlaylists { get; set; }
        public int TotalPodcasts { get; set; }
    }
}
