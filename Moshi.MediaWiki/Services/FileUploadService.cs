using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class FileUploadService : BaseService
{
    public FileUploadService(string connectionString) : base(connectionString) { }

    public FileUpload GetFileUploadById(int fileId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM file_uploads WHERE file_id = @fileId", connection))
            {
                command.Parameters.AddWithValue("@fileId", fileId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new FileUpload
                        {
                            FileId = Convert.ToInt32(reader["file_id"]),
                            Filename = reader["filename"].ToString(),
                            FilePath = reader["file_path"].ToString(),
                            MimeType = reader["mime_type"].ToString(),
                            UploadDate = Convert.ToDateTime(reader["upload_date"]),
                            UserId = Convert.ToInt32(reader["user_id"])
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateFileUpload(FileUpload fileUpload)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                @"INSERT INTO file_uploads (filename, file_path, mime_type, upload_date, user_id) 
                      VALUES (@filename, @filePath, @mimeType, @uploadDate, @userId)",
                connection))
            {
                command.Parameters.AddWithValue("@filename", fileUpload.Filename);
                command.Parameters.AddWithValue("@filePath", fileUpload.FilePath);
                command.Parameters.AddWithValue("@mimeType", fileUpload.MimeType);
                command.Parameters.AddWithValue("@uploadDate", fileUpload.UploadDate);
                command.Parameters.AddWithValue("@userId", fileUpload.UserId);
                command.ExecuteNonQuery();
            }
        }
    }
}