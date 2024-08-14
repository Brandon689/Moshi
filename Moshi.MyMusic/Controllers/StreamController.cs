using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly string _audioBasePath;

        public StreamController(IOptions<DatabaseConfig> databaseConfig, IOptions<AudioConfig> audioConfig)
        {
            _databaseConfig = databaseConfig.Value;
            _audioBasePath = audioConfig.Value.BasePath;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Stream/Song/5
        [HttpGet("Song/{id}")]
        public async Task<IActionResult> StreamSong(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT audio_file_path FROM songs WHERE song_id = @Id";
            var audioFilePath = await connection.QuerySingleOrDefaultAsync<string>(sql, new { Id = id });

            if (string.IsNullOrEmpty(audioFilePath))
            {
                return NotFound("Song not found or audio file path is missing.");
            }

            var fullPath = Path.Combine(_audioBasePath, audioFilePath);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("Audio file not found on the server.");
            }

            var fileStream = System.IO.File.OpenRead(fullPath);
            return File(fileStream, "audio/mpeg", enableRangeProcessing: true);
        }

        // GET: api/Stream/PodcastEpisode/5
        [HttpGet("PodcastEpisode/{id}")]
        public async Task<IActionResult> StreamPodcastEpisode(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT audio_file_path FROM podcast_episodes WHERE episode_id = @Id";
            var audioFilePath = await connection.QuerySingleOrDefaultAsync<string>(sql, new { Id = id });

            if (string.IsNullOrEmpty(audioFilePath))
            {
                return NotFound("Podcast episode not found or audio file path is missing.");
            }

            var fullPath = Path.Combine(_audioBasePath, audioFilePath);
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("Audio file not found on the server.");
            }

            var fileStream = System.IO.File.OpenRead(fullPath);
            return File(fileStream, "audio/mpeg", enableRangeProcessing: true);
        }

        // POST: api/Stream/Song/5/Progress
        [HttpPost("Song/{id}/Progress")]
        public async Task<IActionResult> UpdateSongProgress(int id, [FromBody] ProgressUpdate progress)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT INTO listening_history (user_id, song_id, listened_at, duration_listened)
                        VALUES (@UserId, @SongId, @ListenedAt, @DurationListened)";

            await connection.ExecuteAsync(sql, new
            {
                UserId = progress.UserId,
                SongId = id,
                ListenedAt = DateTime.UtcNow,
                DurationListened = progress.Progress
            });

            return NoContent();
        }

        // POST: api/Stream/PodcastEpisode/5/Progress
        [HttpPost("PodcastEpisode/{id}/Progress")]
        public async Task<IActionResult> UpdatePodcastEpisodeProgress(int id, [FromBody] ProgressUpdate progress)
        {
            using var connection = CreateConnection();
            var sql = @"INSERT OR REPLACE INTO podcast_episode_progress 
                        (user_id, episode_id, progress, last_updated)
                        VALUES (@UserId, @EpisodeId, @Progress, @LastUpdated)";

            await connection.ExecuteAsync(sql, new
            {
                UserId = progress.UserId,
                EpisodeId = id,
                Progress = progress.Progress,
                LastUpdated = DateTime.UtcNow
            });

            return NoContent();
        }
    }

    public class ProgressUpdate
    {
        public int UserId { get; set; }
        public int Progress { get; set; } // in seconds
    }

    public class AudioConfig
    {
        public string BasePath { get; set; }
    }
}