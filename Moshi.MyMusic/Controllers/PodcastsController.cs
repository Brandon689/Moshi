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
    public class PodcastsController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public PodcastsController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Podcasts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Podcast>>> GetPodcasts()
        {
            using var connection = CreateConnection();
            var podcasts = await connection.QueryAsync<Podcast>("SELECT * FROM podcasts ORDER BY title");
            return Ok(podcasts);
        }

        // GET: api/Podcasts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Podcast>> GetPodcast(int id)
        {
            using var connection = CreateConnection();
            var podcast = await connection.QuerySingleOrDefaultAsync<Podcast>(
                "SELECT * FROM podcasts WHERE podcast_id = @Id", new { Id = id });

            if (podcast == null)
            {
                return NotFound();
            }

            return Ok(podcast);
        }

        // POST: api/Podcasts
        [HttpPost]
        public async Task<ActionResult<Podcast>> CreatePodcast(Podcast podcast)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO podcasts (title, description, publisher, language, rss_feed_url)
                        VALUES (@Title, @Description, @Publisher, @Language, @RssFeedUrl);
                        SELECT last_insert_rowid();";

            var id = await connection.ExecuteScalarAsync<int>(sql, podcast);
            podcast.PodcastId = id;

            return CreatedAtAction(nameof(GetPodcast), new { id = podcast.PodcastId }, podcast);
        }

        // PUT: api/Podcasts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePodcast(int id, Podcast podcast)
        {
            if (id != podcast.PodcastId)
            {
                return BadRequest();
            }

            using var connection = CreateConnection();
            var sql = @"UPDATE podcasts 
                        SET title = @Title, description = @Description, publisher = @Publisher, 
                            language = @Language, rss_feed_url = @RssFeedUrl
                        WHERE podcast_id = @PodcastId";

            var affected = await connection.ExecuteAsync(sql, podcast);

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Podcasts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePodcast(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM podcasts WHERE podcast_id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Podcasts/5/Episodes
        [HttpGet("{id}/Episodes")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPodcastEpisodes(int id)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM podcast_episodes 
                        WHERE podcast_id = @PodcastId 
                        ORDER BY release_date DESC";

            var episodes = await connection.QueryAsync(sql, new { PodcastId = id });

            return Ok(episodes);
        }

        // POST: api/Podcasts/5/Subscribe
        [HttpPost("{id}/Subscribe")]
        public async Task<IActionResult> SubscribeToPodcast(int id, [FromBody] int userId)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO podcast_subscriptions (user_id, podcast_id, subscribed_at)
                        VALUES (@UserId, @PodcastId, @SubscribedAt)";

            try
            {
                await connection.ExecuteAsync(sql, new { UserId = userId, PodcastId = id, SubscribedAt = DateTime.UtcNow });
            }
            catch (SqliteException)
            {
                // If the insert fails, it's likely because the subscription already exists
                return BadRequest("User is already subscribed to this podcast");
            }

            return NoContent();
        }

        // DELETE: api/Podcasts/5/Unsubscribe
        [HttpDelete("{id}/Unsubscribe")]
        public async Task<IActionResult> UnsubscribeFromPodcast(int id, [FromBody] int userId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM podcast_subscriptions WHERE user_id = @UserId AND podcast_id = @PodcastId";
            var affected = await connection.ExecuteAsync(sql, new { UserId = userId, PodcastId = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Podcasts/Search?query=example
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Podcast>>> SearchPodcasts(string query)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM podcasts 
                        WHERE title LIKE @Query OR description LIKE @Query OR publisher LIKE @Query
                        ORDER BY title";

            var podcasts = await connection.QueryAsync<Podcast>(sql, new { Query = $"%{query}%" });

            return Ok(podcasts);
        }

        // GET: api/Podcasts/Popular
        [HttpGet("Popular")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPopularPodcasts(int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT p.*, COUNT(ps.user_id) AS subscriber_count
                        FROM podcasts p
                        LEFT JOIN podcast_subscriptions ps ON p.podcast_id = ps.podcast_id
                        GROUP BY p.podcast_id
                        ORDER BY subscriber_count DESC
                        LIMIT @Limit";

            var popularPodcasts = await connection.QueryAsync(sql, new { Limit = limit });

            return Ok(popularPodcasts);
        }
    }
}