using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class PageService : BaseService
{
    public PageService(string connectionString) : base(connectionString) { }

    public Page GetPageById(int pageId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM pages WHERE page_id = @pageId", connection))
            {
                command.Parameters.AddWithValue("@pageId", pageId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Page
                        {
                            PageId = Convert.ToInt32(reader["page_id"]),
                            Title = reader["title"].ToString(),
                            CurrentRevisionId = reader["current_revision_id"] != DBNull.Value ? Convert.ToInt32(reader["current_revision_id"]) : (int?)null,
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            LastModified = Convert.ToDateTime(reader["last_modified"])
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreatePage(Page page)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO pages (title, created_at, last_modified) VALUES (@title, @createdAt, @lastModified)",
                connection))
            {
                command.Parameters.AddWithValue("@title", page.Title);
                command.Parameters.AddWithValue("@createdAt", page.CreatedAt);
                command.Parameters.AddWithValue("@lastModified", page.LastModified);
                command.ExecuteNonQuery();
            }
        }
    }

    // Add more methods as needed (UpdatePage, DeletePage, etc.)
}