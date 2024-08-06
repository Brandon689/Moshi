using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Blog.Models;

namespace Moshi.Blog.Data
{
    public class CommentRepository
    {
        private readonly string _connectionString;

        public CommentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<Comment>("SELECT * FROM Comments WHERE PostId = @PostId", new { PostId = postId });
        }

        public async Task<Comment> CreateComment(Comment comment)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Comments (Content, CreatedAt, PostId, UserId) 
                        VALUES (@Content, @CreatedAt, @PostId, @UserId);
                        SELECT last_insert_rowid();";
            comment.Id = await connection.ExecuteScalarAsync<int>(sql, comment);
            return comment;
        }

        public async Task<Comment> UpdateComment(Comment comment)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"UPDATE Comments 
                        SET Content = @Content 
                        WHERE Id = @Id";
            await connection.ExecuteAsync(sql, comment);
            return comment;
        }

        public async Task<bool> DeleteComment(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync("DELETE FROM Comments WHERE Id = @Id", new { Id = id });
            return affectedRows > 0;
        }
    }
}