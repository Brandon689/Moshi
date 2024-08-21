using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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
            if (limit <= 0 || limit > 100)
            {
                return BadRequest("Limit must be between 1 and 100.");
            }

            if (days <= 0 || days > 365)
            {
                return BadRequest("Days must be between 1 and 365.");
            }

            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        s.song_id AS SongId, 
                        s.title AS Title, 
                        a.name AS ArtistName, 
                        COUNT(*) AS PlayCount
                    FROM listening_history lh
                    JOIN songs s ON lh.song_id = s.song_id
                    JOIN song_artists sa ON s.song_id = sa.song_id
                    JOIN artists a ON sa.artist_id = a.artist_id
                    WHERE lh.listened_at >= @StartDate
                    GROUP BY s.song_id
                    ORDER BY PlayCount DESC
                    LIMIT @Limit";

                var startDate = DateTime.UtcNow.AddDays(-days);
                var topSongs = await connection.QueryAsync<TopSong>(sql, new { StartDate = startDate, Limit = limit });

                return Ok(topSongs);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Reporting/TopArtists
        [HttpGet("TopArtists")]
        public async Task<ActionResult<IEnumerable<TopArtist>>> GetTopArtists(int limit = 10, int days = 30)
        {
            if (limit <= 0 || limit > 100)
            {
                return BadRequest("Limit must be between 1 and 100.");
            }

            if (days <= 0 || days > 365)
            {
                return BadRequest("Days must be between 1 and 365.");
            }

            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        a.artist_id AS ArtistId, 
                        a.name AS Name, 
                        COUNT(DISTINCT lh.user_id) AS ListenerCount
                    FROM listening_history lh
                    JOIN songs s ON lh.song_id = s.song_id
                    JOIN song_artists sa ON s.song_id = sa.song_id
                    JOIN artists a ON sa.artist_id = a.artist_id
                    WHERE lh.listened_at >= @StartDate
                    GROUP BY a.artist_id
                    ORDER BY ListenerCount DESC
                    LIMIT @Limit";

                var startDate = DateTime.UtcNow.AddDays(-days);
                var topArtists = await connection.QueryAsync<TopArtist>(sql, new { StartDate = startDate, Limit = limit });

                return Ok(topArtists);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Reporting/UserGrowth
        [HttpGet("UserGrowth")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserGrowthData>>> GetUserGrowth(int months = 12)
        {
            if (months <= 0 || months > 60)
            {
                return BadRequest("Months must be between 1 and 60.");
            }

            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        strftime('%Y-%m', created_at) AS Month,
                        COUNT(*) AS NewUsers
                    FROM users
                    WHERE created_at >= @StartDate
                    GROUP BY strftime('%Y-%m', created_at)
                    ORDER BY Month";

                var startDate = DateTime.UtcNow.AddMonths(-months);
                var userGrowth = await connection.QueryAsync<UserGrowthData>(sql, new { StartDate = startDate });

                return Ok(userGrowth);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Reporting/GenrePopularity
        [HttpGet("GenrePopularity")]
        public async Task<ActionResult<IEnumerable<GenrePopularity>>> GetGenrePopularity()
        {
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        g.name AS Genre, 
                        COUNT(*) AS ListenCount
                    FROM listening_history lh
                    JOIN songs s ON lh.song_id = s.song_id
                    JOIN song_genres sg ON s.song_id = sg.song_id
                    JOIN genres g ON sg.genre_id = g.genre_id
                    GROUP BY g.genre_id
                    ORDER BY ListenCount DESC";

                var genrePopularity = await connection.QueryAsync<GenrePopularity>(sql);

                return Ok(genrePopularity);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Reporting/UserActivity
        [HttpGet("UserActivity")]
        public async Task<ActionResult<UserActivity>> GetUserActivity()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM listening_history WHERE user_id = @UserId) AS TotalListens,
                        (SELECT COUNT(DISTINCT song_id) FROM listening_history WHERE user_id = @UserId) AS UniqueSongs,
                        (SELECT COUNT(*) FROM playlists WHERE user_id = @UserId) AS PlaylistCount,
                        (SELECT COUNT(*) FROM user_followers WHERE follower_id = @UserId) AS FollowingCount,
                        (SELECT COUNT(*) FROM user_followers WHERE followed_id = @UserId) AS FollowerCount";

                var userActivity = await connection.QuerySingleOrDefaultAsync<UserActivity>(sql, new { UserId = userId });

                return Ok(userActivity);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/Reporting/SystemStats
        [HttpGet("SystemStats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SystemStats>> GetSystemStats()
        {
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM users) AS TotalUsers,
                        (SELECT COUNT(*) FROM songs) AS TotalSongs,
                        (SELECT COUNT(*) FROM artists) AS TotalArtists,
                        (SELECT COUNT(*) FROM albums) AS TotalAlbums,
                        (SELECT COUNT(*) FROM playlists) AS TotalPlaylists,
                        (SELECT COUNT(*) FROM podcasts) AS TotalPodcasts";

                var systemStats = await connection.QuerySingleOrDefaultAsync<SystemStats>(sql);

                return Ok(systemStats);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}