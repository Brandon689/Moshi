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
    public class UserProfileController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public UserProfileController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/UserProfile
        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetUserProfile()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = "SELECT * FROM users WHERE user_id = @UserId";
            var user = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { UserId = userId });

            if (user == null)
            {
                return NotFound();
            }

            // Don't return sensitive information
            user.PasswordHash = null;

            return user;
        }

        // PUT: api/UserProfile
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile(UserProfileUpdate model)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = @"UPDATE users 
                        SET username = @Username, email = @Email, date_of_birth = @DateOfBirth, 
                            country = @Country
                        WHERE user_id = @UserId";

            var affected = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                model.Username,
                model.Email,
                model.DateOfBirth,
                model.Country
            });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PUT: api/UserProfile/ChangePassword
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(PasswordChange model)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();

            // Verify current password
            var currentHash = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT password_hash FROM users WHERE user_id = @UserId", new { UserId = userId });

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, currentHash))
            {
                return BadRequest("Current password is incorrect.");
            }

            // Update password
            var newHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            var sql = "UPDATE users SET password_hash = @PasswordHash WHERE user_id = @UserId";
            await connection.ExecuteAsync(sql, new { UserId = userId, PasswordHash = newHash });

            return NoContent();
        }

        // DELETE: api/UserProfile
        [HttpDelete]
        public async Task<IActionResult> DeleteUserProfile()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = "DELETE FROM users WHERE user_id = @UserId";
            var affected = await connection.ExecuteAsync(sql, new { UserId = userId });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/UserProfile/Preferences
        [HttpGet("Preferences")]
        public async Task<ActionResult<UserPreferences>> GetUserPreferences()
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = "SELECT * FROM user_preferences WHERE user_id = @UserId";
            var preferences = await connection.QuerySingleOrDefaultAsync<UserPreferences>(sql, new { UserId = userId });

            if (preferences == null)
            {
                return NotFound();
            }

            return preferences;
        }

        // PUT: api/UserProfile/Preferences
        [HttpPut("Preferences")]
        public async Task<IActionResult> UpdateUserPreferences(UserPreferences model)
        {
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            using var connection = CreateConnection();
            var sql = @"INSERT OR REPLACE INTO user_preferences 
                        (user_id, language, theme, notifications_enabled)
                        VALUES (@UserId, @Language, @Theme, @NotificationsEnabled)";

            await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                model.Language,
                model.Theme,
                model.NotificationsEnabled
            });

            return NoContent();
        }
    }

    public class UserProfile
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public bool PremiumStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class UserProfileUpdate
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
    }

    public class PasswordChange
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserPreferences
    {
        public int UserId { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public bool NotificationsEnabled { get; set; }
    }
}