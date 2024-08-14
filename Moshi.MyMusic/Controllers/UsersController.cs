using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System.Data;
namespace Moshi.MyMusic.Controllers;

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
        using var connection = CreateConnection();
        var users = await connection.QueryAsync<User>("SELECT * FROM users");
        return Ok(users);
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        using var connection = CreateConnection();
        var user = await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM users WHERE user_id = @Id", new { Id = id });

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO users (username, email, password_hash, date_of_birth, country, premium_status, created_at)
                    VALUES (@Username, @Email, @PasswordHash, @DateOfBirth, @Country, @PremiumStatus, @CreatedAt);
                    SELECT last_insert_rowid();";

        user.CreatedAt = DateTime.UtcNow;
        var id = await connection.ExecuteScalarAsync<int>(sql, user);
        user.UserId = id;

        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.UserId)
        {
            return BadRequest();
        }

        using var connection = CreateConnection();
        var sql = @"UPDATE users 
                    SET username = @Username, email = @Email, password_hash = @PasswordHash, 
                        date_of_birth = @DateOfBirth, country = @Country, premium_status = @PremiumStatus
                    WHERE user_id = @UserId";

        var affected = await connection.ExecuteAsync(sql, user);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
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
}