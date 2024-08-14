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
    public class UserFollowersController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public UserFollowersController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // POST: api/UserFollowers/FollowUser
        [HttpPost("FollowUser")]
        public async Task<IActionResult> FollowUser(UserFollower userFollower)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO user_followers (follower_id, followed_id, followed_at)
                        VALUES (@FollowerId, @FollowedId, @FollowedAt)";

            userFollower.FollowedAt = DateTime.UtcNow;

            try
            {
                await connection.ExecuteAsync(sql, userFollower);
            }
            catch (SqliteException)
            {
                // If the insert fails, it's likely because the relationship already exists
                return BadRequest("User is already following this user");
            }

            return NoContent();
        }

        // DELETE: api/UserFollowers/UnfollowUser/5/10
        [HttpDelete("UnfollowUser/{followerId}/{followedId}")]
        public async Task<IActionResult> UnfollowUser(int followerId, int followedId)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM user_followers 
                        WHERE follower_id = @FollowerId AND followed_id = @FollowedId";

            var affected = await connection.ExecuteAsync(sql, new { FollowerId = followerId, FollowedId = followedId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/UserFollowers/Followers/5
        [HttpGet("Followers/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetFollowers(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT u.user_id, u.username, u.email, uf.followed_at
                        FROM user_followers uf
                        JOIN users u ON uf.follower_id = u.user_id
                        WHERE uf.followed_id = @UserId
                        ORDER BY uf.followed_at DESC";

            var followers = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(followers);
        }

        // GET: api/UserFollowers/Following/5
        [HttpGet("Following/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetFollowing(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT u.user_id, u.username, u.email, uf.followed_at
                        FROM user_followers uf
                        JOIN users u ON uf.followed_id = u.user_id
                        WHERE uf.follower_id = @UserId
                        ORDER BY uf.followed_at DESC";

            var following = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(following);
        }

        // GET: api/UserFollowers/IsFollowing/5/10
        [HttpGet("IsFollowing/{followerId}/{followedId}")]
        public async Task<ActionResult<bool>> IsFollowing(int followerId, int followedId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT COUNT(*) FROM user_followers 
                        WHERE follower_id = @FollowerId AND followed_id = @FollowedId";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { FollowerId = followerId, FollowedId = followedId });

            return Ok(count > 0);
        }

        // POST: api/UserFollowers/FollowArtist
        [HttpPost("FollowArtist")]
        public async Task<IActionResult> FollowArtist([FromBody] ArtistFollower artistFollower)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO artist_followers (user_id, artist_id, followed_at)
                        VALUES (@UserId, @ArtistId, @FollowedAt)";

            artistFollower.FollowedAt = DateTime.UtcNow;

            try
            {
                await connection.ExecuteAsync(sql, artistFollower);
            }
            catch (SqliteException)
            {
                // If the insert fails, it's likely because the relationship already exists
                return BadRequest("User is already following this artist");
            }

            return NoContent();
        }

        // DELETE: api/UserFollowers/UnfollowArtist/5/10
        [HttpDelete("UnfollowArtist/{userId}/{artistId}")]
        public async Task<IActionResult> UnfollowArtist(int userId, int artistId)
        {
            using var connection = CreateConnection();
            var sql = @"DELETE FROM artist_followers 
                        WHERE user_id = @UserId AND artist_id = @ArtistId";

            var affected = await connection.ExecuteAsync(sql, new { UserId = userId, ArtistId = artistId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/UserFollowers/FollowedArtists/5
        [HttpGet("FollowedArtists/{userId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetFollowedArtists(int userId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT a.artist_id, a.name, a.country, af.followed_at
                        FROM artist_followers af
                        JOIN artists a ON af.artist_id = a.artist_id
                        WHERE af.user_id = @UserId
                        ORDER BY af.followed_at DESC";

            var followedArtists = await connection.QueryAsync(sql, new { UserId = userId });
            return Ok(followedArtists);
        }

        // GET: api/UserFollowers/ArtistFollowers/5
        [HttpGet("ArtistFollowers/{artistId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetArtistFollowers(int artistId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT u.user_id, u.username, u.email, af.followed_at
                        FROM artist_followers af
                        JOIN users u ON af.user_id = u.user_id
                        WHERE af.artist_id = @ArtistId
                        ORDER BY af.followed_at DESC";

            var artistFollowers = await connection.QueryAsync(sql, new { ArtistId = artistId });
            return Ok(artistFollowers);
        }
    }

    public class ArtistFollower
    {
        public int UserId { get; set; }
        public int ArtistId { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}