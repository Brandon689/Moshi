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
    public class PodcastEpisodesController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public PodcastEpisodesController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/PodcastEpisodes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PodcastEpisode>>> GetPodcastEpisodes()
        {
            using var connection = CreateConnection();
            var episodes = await connection.QueryAsync<PodcastEpisode>(
                "SELECT * FROM podcast_episodes ORDER BY release_date DESC");
            return Ok(episodes);
        }

        // GET: api/PodcastEpisodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PodcastEpisode>> GetPodcastEpisode(int id)
        {
            using var connection = CreateConnection();
            var episode = await connection.QuerySingleOrDefaultAsync<PodcastEpisode>(
                "SELECT * FROM podcast_episodes WHERE episode_id = @Id", new { Id = id });

            if (episode == null)
            {
                return NotFound();
            }

            return Ok(episode);
        }

        // POST: api/PodcastEpisodes
        [HttpPost]
        public async Task<ActionResult<PodcastEpisode>> CreatePodcastEpisode(PodcastEpisode episode)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO podcast_episodes (podcast_id, title, description, duration, release_date, audio_file_path)
                        VALUES (@PodcastId, @Title, @Description, @Duration, @ReleaseDate, @AudioFilePath);
                        SELECT last_insert_rowid();";

            var id = await connection.ExecuteScalarAsync<int>(sql, episode);
            episode.EpisodeId = id;

            return CreatedAtAction(nameof(GetPodcastEpisode), new { id = episode.EpisodeId }, episode);
        }

        // PUT: api/PodcastEpisodes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePodcastEpisode(int id, PodcastEpisode episode)
        {
            if (id != episode.EpisodeId)
            {
                return BadRequest();
            }

            using var connection = CreateConnection();
            var sql = @"UPDATE podcast_episodes 
                        SET podcast_id = @PodcastId, title = @Title, description = @Description, 
                            duration = @Duration, release_date = @ReleaseDate, audio_file_path = @AudioFilePath
                        WHERE episode_id = @EpisodeId";

            var affected = await connection.ExecuteAsync(sql, episode);

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/PodcastEpisodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePodcastEpisode(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM podcast_episodes WHERE episode_id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/PodcastEpisodes/ByPodcast/5
        [HttpGet("ByPodcast/{podcastId}")]
        public async Task<ActionResult<IEnumerable<PodcastEpisode>>> GetEpisodesByPodcast(int podcastId)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT * FROM podcast_episodes 
                        WHERE podcast_id = @PodcastId 
                        ORDER BY release_date DESC";

            var episodes = await connection.QueryAsync<PodcastEpisode>(sql, new { PodcastId = podcastId });

            return Ok(episodes);
        }

        // GET: api/PodcastEpisodes/Recent
        [HttpGet("Recent")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetRecentEpisodes(int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT pe.*, p.title AS podcast_title
                        FROM podcast_episodes pe
                        JOIN podcasts p ON pe.podcast_id = p.podcast_id
                        ORDER BY pe.release_date DESC
                        LIMIT @Limit";

            var recentEpisodes = await connection.QueryAsync(sql, new { Limit = limit });

            return Ok(recentEpisodes);
        }

        // GET: api/PodcastEpisodes/Search?query=example
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchPodcastEpisodes(string query)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT pe.*, p.title AS podcast_title
                        FROM podcast_episodes pe
                        JOIN podcasts p ON pe.podcast_id = p.podcast_id
                        WHERE pe.title LIKE @Query OR pe.description LIKE @Query
                        ORDER BY pe.release_date DESC";

            var episodes = await connection.QueryAsync(sql, new { Query = $"%{query}%" });

            return Ok(episodes);
        }

        // POST: api/PodcastEpisodes/5/Listen
        [HttpPost("{id}/Listen")]
        public async Task<IActionResult> RecordEpisodeListen(int id, [FromBody] int userId)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO podcast_episode_listens (user_id, episode_id, listened_at)
                        VALUES (@UserId, @EpisodeId, @ListenedAt)";

            await connection.ExecuteAsync(sql, new { UserId = userId, EpisodeId = id, ListenedAt = DateTime.UtcNow });

            return NoContent();
        }

        // GET: api/PodcastEpisodes/Popular
        [HttpGet("Popular")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPopularEpisodes(int limit = 10)
        {
            using var connection = CreateConnection();
            var sql = @"SELECT pe.*, p.title AS podcast_title, COUNT(pel.user_id) AS listen_count
                        FROM podcast_episodes pe
                        JOIN podcasts p ON pe.podcast_id = p.podcast_id
                        LEFT JOIN podcast_episode_listens pel ON pe.episode_id = pel.episode_id
                        GROUP BY pe.episode_id
                        ORDER BY listen_count DESC
                        LIMIT @Limit";

            var popularEpisodes = await connection.QueryAsync(sql, new { Limit = limit });

            return Ok(popularEpisodes);
        }
    }
}