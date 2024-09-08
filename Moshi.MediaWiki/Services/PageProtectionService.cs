using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class PageProtectionService : BaseService
{
    public PageProtectionService(string connectionString) : base(connectionString) { }

    public PageProtection GetPageProtectionById(int protectionId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM page_protection WHERE protection_id = @protectionId", connection))
            {
                command.Parameters.AddWithValue("@protectionId", protectionId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PageProtection
                        {
                            ProtectionId = Convert.ToInt32(reader["protection_id"]),
                            PageId = Convert.ToInt32(reader["page_id"]),
                            ProtectionType = reader["protection_type"].ToString(),
                            GroupId = Convert.ToInt32(reader["group_id"]),
                            ExpiryDate = reader["expiry_date"] != DBNull.Value ? Convert.ToDateTime(reader["expiry_date"]) : (DateTime?)null
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreatePageProtection(PageProtection pageProtection)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                @"INSERT INTO page_protection (page_id, protection_type, group_id, expiry_date) 
                      VALUES (@pageId, @protectionType, @groupId, @expiryDate)",
                connection))
            {
                command.Parameters.AddWithValue("@pageId", pageProtection.PageId);
                command.Parameters.AddWithValue("@protectionType", pageProtection.ProtectionType);
                command.Parameters.AddWithValue("@groupId", pageProtection.GroupId);
                command.Parameters.AddWithValue("@expiryDate", (object)pageProtection.ExpiryDate ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }
    }

    public PageProtection GetPageProtectionForPage(int pageId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM page_protection WHERE page_id = @pageId", connection))
            {
                command.Parameters.AddWithValue("@pageId", pageId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PageProtection
                        {
                            ProtectionId = Convert.ToInt32(reader["protection_id"]),
                            PageId = Convert.ToInt32(reader["page_id"]),
                            ProtectionType = reader["protection_type"].ToString(),
                            GroupId = Convert.ToInt32(reader["group_id"]),
                            ExpiryDate = reader["expiry_date"] != DBNull.Value ? Convert.ToDateTime(reader["expiry_date"]) : (DateTime?)null
                        };
                    }
                }
            }
        }
        return null;
    }
}