using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class RevisionService : BaseService
{
    public RevisionService(string connectionString) : base(connectionString) { }

    public Revision GetRevisionById(int revisionId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM revisions WHERE revision_id = @revisionId", connection))
            {
                command.Parameters.AddWithValue("@revisionId", revisionId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Revision
                        {
                            RevisionId = Convert.ToInt32(reader["revision_id"]),
                            PageId = Convert.ToInt32(reader["page_id"]),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Content = reader["content"].ToString(),
                            Comment = reader["comment"].ToString(),
                            Timestamp = Convert.ToDateTime(reader["timestamp"])
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateRevision(Revision revision)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO revisions (page_id, user_id, content, comment, timestamp) VALUES (@pageId, @userId, @content, @comment, @timestamp)",
                connection))
            {
                command.Parameters.AddWithValue("@pageId", revision.PageId);
                command.Parameters.AddWithValue("@userId", revision.UserId);
                command.Parameters.AddWithValue("@content", revision.Content);
                command.Parameters.AddWithValue("@comment", revision.Comment);
                command.Parameters.AddWithValue("@timestamp", revision.Timestamp);
                command.ExecuteNonQuery();
            }

            // Update the current_revision_id in the pages table
            using (var updateCommand = new SQLiteCommand(
                "UPDATE pages SET current_revision_id = last_insert_rowid(), last_modified = @timestamp WHERE page_id = @pageId",
                connection))
            {
                updateCommand.Parameters.AddWithValue("@timestamp", revision.Timestamp);
                updateCommand.Parameters.AddWithValue("@pageId", revision.PageId);
                updateCommand.ExecuteNonQuery();
            }
        }
    }

    // Add more methods as needed (GetRevisionsByPageId, etc.)
}