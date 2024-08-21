using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moshi.MyMusic.Data;
using Moshi.MyMusic.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Moshi.MyMusic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly JwtConfig _jwtConfig;

        public AuthController(IOptions<DatabaseConfig> databaseConfig, IOptions<JwtConfig> jwtConfig)
        {
            _databaseConfig = databaseConfig.Value;
            _jwtConfig = jwtConfig.Value;
        }

        private IDbConnection CreateConnection()
        {
            return new SqliteConnection(_databaseConfig.ConnectionString);
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistration model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var connection = CreateConnection();

            // Check if user already exists
            var existingUser = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE username = @Username OR email = @Email",
                new { model.Username, model.Email });

            if (existingUser != null)
            {
                return BadRequest("Username or email already exists.");
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Insert new user
            var sql = @"INSERT INTO users (username, email, password_hash, date_of_birth, country, created_at)
                        VALUES (@Username, @Email, @PasswordHash, @DateOfBirth, @Country, @CreatedAt);
                        SELECT last_insert_rowid();";

            var userId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                model.Username,
                model.Email,
                PasswordHash = passwordHash,
                model.DateOfBirth,
                model.Country,
                CreatedAt = DateTime.UtcNow
            });

            return Ok(new { UserId = userId, Message = "User registered successfully." });
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin model)
        {
            using var connection = CreateConnection();

            var user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM users WHERE username = @Username OR email = @Username",
                new { Username = model.Username });

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);

            // Update last login
            await connection.ExecuteAsync(
                "UPDATE users SET last_login = @LastLogin WHERE user_id = @UserId",
                new { LastLogin = DateTime.UtcNow, UserId = user.UserId });

            return Ok(new { Token = token, UserId = user.UserId });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
