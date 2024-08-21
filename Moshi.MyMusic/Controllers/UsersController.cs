using Dapper;
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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;

        public UsersController(IOptions<DatabaseConfig> databaseConfig)
        {
            _databaseConfig = databaseConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                using var connection = CreateConnection();
                var users = await connection.QueryAsync<User>(@"
                    SELECT 
                        user_id AS UserId, 
                        username AS Username, 
                        email AS Email, 
                        password_hash AS PasswordHash, 
                        date_of_birth AS DateOfBirth, 
                        country AS Country, 
                        premium_status AS PremiumStatus, 
                        created_at AS CreatedAt, 
                        last_login AS LastLogin 
                    FROM users");
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                using var connection = CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<User>(@"
                    SELECT 
                        user_id AS UserId, 
                        username AS Username, 
                        email AS Email, 
                        password_hash AS PasswordHash, 
                        date_of_birth AS DateOfBirth, 
                        country AS Country, 
                        premium_status AS PremiumStatus, 
                        created_at AS CreatedAt, 
                        last_login AS LastLogin 
                    FROM users 
                    WHERE user_id = @Id", new { Id = id });

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    INSERT INTO users (username, email, password_hash, date_of_birth, country, premium_status, created_at)
                    VALUES (@Username, @Email, @PasswordHash, @DateOfBirth, @Country, @PremiumStatus, @CreatedAt);
                    SELECT last_insert_rowid();";

                user.CreatedAt = DateTime.UtcNow;
                var id = await connection.ExecuteScalarAsync<int>(sql, user);
                user.UserId = id;

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while creating the user.");
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            try
            {
                using var connection = CreateConnection();
                var sql = @"
                    UPDATE users 
                    SET username = @Username, 
                        email = @Email, 
                        password_hash = @PasswordHash, 
                        date_of_birth = @DateOfBirth, 
                        country = @Country, 
                        premium_status = @PremiumStatus
                    WHERE user_id = @UserId";

                var affected = await connection.ExecuteAsync(sql, user);

                if (affected == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                using var connection = CreateConnection();
                var sql = "DELETE FROM users WHERE user_id = @Id";
                var affected = await connection.ExecuteAsync(sql, new { Id = id });

                if (affected == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }
    }
}