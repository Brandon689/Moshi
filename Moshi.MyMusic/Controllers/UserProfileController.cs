using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System;
using System.Data;
using System.Threading.Tasks;

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
            try
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        user_id AS UserId, 
                        username AS Username, 
                        email AS Email, 
                        date_of_birth AS DateOfBirth, 
                        country AS Country, 
                        premium_status AS PremiumStatus, 
                        created_at AS CreatedAt, 
                        last_login AS LastLogin 
                    FROM users 
                    WHERE user_id = @UserId";
                var user = await connection.QuerySingleOrDefaultAsync<UserProfile>(sql, new { UserId = userId });

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving the user profile.");
            }
        }

        // PUT: api/UserProfile
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile(UserProfileUpdate model)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                using var connection = CreateConnection();
                var sql = @"
                    UPDATE users 
                    SET username = @Username, 
                        email = @Email, 
                        date_of_birth = @DateOfBirth, 
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
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while updating the user profile.");
            }
        }

        // PUT: api/UserProfile/ChangePassword
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(PasswordChange model)
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }

        // DELETE: api/UserProfile
        [HttpDelete]
        public async Task<IActionResult> DeleteUserProfile()
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while deleting the user profile.");
            }
        }

        // GET: api/UserProfile/Preferences
        [HttpGet("Preferences")]
        public async Task<ActionResult<UserPreferences>> GetUserPreferences()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                using var connection = CreateConnection();
                var sql = @"
                    SELECT 
                        user_id AS UserId, 
                        language AS Language, 
                        theme AS Theme, 
                        notifications_enabled AS NotificationsEnabled 
                    FROM user_preferences 
                    WHERE user_id = @UserId";
                var preferences = await connection.QuerySingleOrDefaultAsync<UserPreferences>(sql, new { UserId = userId });

                if (preferences == null)
                {
                    return NotFound();
                }

                return preferences;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving user preferences.");
            }
        }

        // PUT: api/UserProfile/Preferences
        [HttpPut("Preferences")]
        public async Task<IActionResult> UpdateUserPreferences(UserPreferences model)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                using var connection = CreateConnection();
                var sql = @"
                    INSERT OR REPLACE INTO user_preferences 
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
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while updating user preferences.");
            }
        }
    }
}