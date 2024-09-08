using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class CategoryService : BaseService
{
    public CategoryService(string connectionString) : base(connectionString) { }

    public Category GetCategoryById(int categoryId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM categories WHERE category_id = @categoryId", connection))
            {
                command.Parameters.AddWithValue("@categoryId", categoryId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Category
                        {
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateCategory(Category category)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO categories (name, description) VALUES (@name, @description)",
                connection))
            {
                command.Parameters.AddWithValue("@name", category.Name);
                command.Parameters.AddWithValue("@description", category.Description);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Category> GetCategoriesForPage(int pageId)
    {
        var categories = new List<Category>();
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                @"SELECT c.* FROM categories c
                      JOIN page_categories pc ON c.category_id = pc.category_id
                      WHERE pc.page_id = @pageId",
                connection))
            {
                command.Parameters.AddWithValue("@pageId", pageId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryId = Convert.ToInt32(reader["category_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        });
                    }
                }
            }
        }
        return categories;
    }
}