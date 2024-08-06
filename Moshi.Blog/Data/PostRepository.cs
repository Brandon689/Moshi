using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Blog.Models;

namespace Moshi.Blog.Data
{
    public class PostRepository
    {
        private readonly string _connectionString;

        public PostRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<Post>("SELECT * FROM Posts");
        }

        public async Task<Post> GetPostById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Post>("SELECT * FROM Posts WHERE Id = @Id", new { Id = id });
        }

        public async Task<Post> CreatePost(Post post)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Posts (Title, Content, CreatedAt, AuthorId, CategoryId) 
                        VALUES (@Title, @Content, @CreatedAt, @AuthorId, @CategoryId);
                        SELECT last_insert_rowid();";
            post.Id = await connection.ExecuteScalarAsync<int>(sql, post);
            return post;
        }

        public async Task<Post> UpdatePost(Post post)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"UPDATE Posts 
                        SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt, 
                            AuthorId = @AuthorId, CategoryId = @CategoryId 
                        WHERE Id = @Id";
            await connection.ExecuteAsync(sql, post);
            return post;
        }

        public async Task<bool> DeletePost(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync("DELETE FROM Posts WHERE Id = @Id", new { Id = id });
            return affectedRows > 0;
        }
    }
}