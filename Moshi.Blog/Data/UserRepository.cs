using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Blog.Models;

namespace Moshi.Blog.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<User>("SELECT * FROM Users");
        }

        public async Task<User> GetUserById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });
        }

        public async Task<User> CreateUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, CreatedAt, IsAdmin) 
                        VALUES (@Username, @Email, @PasswordHash, @CreatedAt, @IsAdmin);
                        SELECT last_insert_rowid();";
            user.Id = await connection.ExecuteScalarAsync<int>(sql, user);
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"UPDATE Users 
                        SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, IsAdmin = @IsAdmin 
                        WHERE Id = @Id";
            await connection.ExecuteAsync(sql, user);
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
            return affectedRows > 0;
        }
    }
}