using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class UserService : BaseService
{
    public UserService(string connectionString) : base(connectionString) { }

    public User GetUserById(int userId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM users WHERE user_id = @userId", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            PasswordHash = reader["password_hash"].ToString(),
                            RegistrationDate = Convert.ToDateTime(reader["registration_date"]),
                            LastLogin = reader["last_login"] != DBNull.Value ? Convert.ToDateTime(reader["last_login"]) : (DateTime?)null
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateUser(User user)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO users (username, email, password_hash, registration_date) VALUES (@username, @email, @passwordHash, @registrationDate)",
                connection))
            {
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@registrationDate", user.RegistrationDate);
                command.ExecuteNonQuery();
            }
        }
    }

    // Add more methods as needed (UpdateUser, DeleteUser, etc.)
}