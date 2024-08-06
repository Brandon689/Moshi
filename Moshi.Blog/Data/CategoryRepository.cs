using Dapper;
using Microsoft.Data.Sqlite;
using Moshi.Blog.Models;

namespace Moshi.Blog.Data
{
    public class CategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Categories (Name, Description) 
                        VALUES (@Name, @Description);
                        SELECT last_insert_rowid();";
            category.Id = await connection.ExecuteScalarAsync<int>(sql, category);
            return category;
        }
    }
}
