using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class LinkService : BaseService
{
    public LinkService(string connectionString) : base(connectionString) { }

    public Link GetLinkById(int linkId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM links WHERE link_id = @linkId", connection))
            {
                command.Parameters.AddWithValue("@linkId", linkId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Link
                        {
                            LinkId = Convert.ToInt32(reader["link_id"]),
                            SourcePageId = Convert.ToInt32(reader["source_page_id"]),
                            TargetPageId = Convert.ToInt32(reader["target_page_id"]),
                            LinkText = reader["link_text"].ToString()
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateLink(Link link)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO links (source_page_id, target_page_id, link_text) VALUES (@sourcePageId, @targetPageId, @linkText)",
                connection))
            {
                command.Parameters.AddWithValue("@sourcePageId", link.SourcePageId);
                command.Parameters.AddWithValue("@targetPageId", link.TargetPageId);
                command.Parameters.AddWithValue("@linkText", link.LinkText);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Link> GetLinksForPage(int pageId)
    {
        var links = new List<Link>();
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "SELECT * FROM links WHERE source_page_id = @pageId",
                connection))
            {
                command.Parameters.AddWithValue("@pageId", pageId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        links.Add(new Link
                        {
                            LinkId = Convert.ToInt32(reader["link_id"]),
                            SourcePageId = Convert.ToInt32(reader["source_page_id"]),
                            TargetPageId = Convert.ToInt32(reader["target_page_id"]),
                            LinkText = reader["link_text"].ToString()
                        });
                    }
                }
            }
        }
        return links;
    }
}