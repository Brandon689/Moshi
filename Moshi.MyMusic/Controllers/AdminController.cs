using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using System.Data;

namespace Moshi.MyMusic.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public AdminController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Admin/Users
        [HttpGet("Users")]
        public async Task<ActionResult<IEnumerable<UserAdminView>>> GetUsers(int page = 1, int pageSize = 20)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT user_id, username, email, date_of_birth, country, premium_status, created_at, last_login
                FROM users
                ORDER BY created_at DESC
                LIMIT @PageSize OFFSET @Offset";

            var users = await connection.QueryAsync<UserAdminView>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            return Ok(users);
        }

        // GET: api/Admin/Users/5
        [HttpGet("Users/{id}")]
        public async Task<ActionResult<UserAdminView>> GetUser(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT user_id, username, email, date_of_birth, country, premium_status, created_at, last_login
                FROM users
                WHERE user_id = @UserId";

            var user = await connection.QuerySingleOrDefaultAsync<UserAdminView>(sql, new { UserId = id });

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Admin/Users/5/TogglePremium
        [HttpPut("Users/{id}/TogglePremium")]
        public async Task<IActionResult> ToggleUserPremiumStatus(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                UPDATE users
                SET premium_status = NOT premium_status
                WHERE user_id = @UserId";

            var affected = await connection.ExecuteAsync(sql, new { UserId = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Admin/Users/5
        [HttpDelete("Users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM users WHERE user_id = @UserId";

            var affected = await connection.ExecuteAsync(sql, new { UserId = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Admin/ContentModeration
        [HttpGet("ContentModeration")]
        public async Task<ActionResult<IEnumerable<ContentModerationItem>>> GetContentForModeration(int page = 1, int pageSize = 20)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT 'song' as content_type, song_id as content_id, title, 'pending' as status
                FROM songs
                WHERE moderation_status = 'pending'
                UNION ALL
                SELECT 'playlist' as content_type, playlist_id as content_id, name as title, 'pending' as status
                FROM playlists
                WHERE moderation_status = 'pending'
                ORDER BY content_type, title
                LIMIT @PageSize OFFSET @Offset";

            var content = await connection.QueryAsync<ContentModerationItem>(sql, new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            return Ok(content);
        }

        // PUT: api/Admin/ContentModeration/5
        [HttpPut("ContentModeration/{id}")]
        public async Task<IActionResult> ModerateContent(int id, [FromBody] ContentModerationUpdate update)
        {
            using var connection = CreateConnection();
            string sql;

            if (update.ContentType == "song")
            {
                sql = "UPDATE songs SET moderation_status = @Status WHERE song_id = @ContentId";
            }
            else if (update.ContentType == "playlist")
            {
                sql = "UPDATE playlists SET moderation_status = @Status WHERE playlist_id = @ContentId";
            }
            else
            {
                return BadRequest("Invalid content type");
            }

            var affected = await connection.ExecuteAsync(sql, new { Status = update.Status, ContentId = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Admin/SystemConfig
        [HttpGet("SystemConfig")]
        public async Task<ActionResult<SystemConfig>> GetSystemConfig()
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM system_config LIMIT 1";

            var config = await connection.QuerySingleOrDefaultAsync<SystemConfig>(sql);

            if (config == null)
            {
                return NotFound();
            }

            return Ok(config);
        }

        // PUT: api/Admin/SystemConfig
        [HttpPut("SystemConfig")]
        public async Task<IActionResult> UpdateSystemConfig([FromBody] SystemConfig config)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT OR REPLACE INTO system_config 
                (max_playlist_size, max_library_size, allow_explicit_content, maintenance_mode)
                VALUES (@MaxPlaylistSize, @MaxLibrarySize, @AllowExplicitContent, @MaintenanceMode)";

            await connection.ExecuteAsync(sql, config);

            return NoContent();
        }
    }

    public class UserAdminView
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public bool PremiumStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class ContentModerationItem
    {
        public string ContentType { get; set; }
        public int ContentId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }

    public class ContentModerationUpdate
    {
        public string ContentType { get; set; }
        public string Status { get; set; }
    }

    public class SystemConfig
    {
        public int MaxPlaylistSize { get; set; }
        public int MaxLibrarySize { get; set; }
        public bool AllowExplicitContent { get; set; }
        public bool MaintenanceMode { get; set; }
    }
}