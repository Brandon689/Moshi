using Moshi.MediaWiki.Models;
using System.Data.SQLite;

namespace Moshi.MediaWiki.Services;

public class UserGroupService : BaseService
{
    public UserGroupService(string connectionString) : base(connectionString) { }

    public UserGroup GetUserGroupById(int groupId)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand("SELECT * FROM user_groups WHERE group_id = @groupId", connection))
            {
                command.Parameters.AddWithValue("@groupId", groupId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserGroup
                        {
                            GroupId = Convert.ToInt32(reader["group_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        };
                    }
                }
            }
        }
        return null;
    }

    public void CreateUserGroup(UserGroup userGroup)
    {
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                "INSERT INTO user_groups (name, description) VALUES (@name, @description)",
                connection))
            {
                command.Parameters.AddWithValue("@name", userGroup.Name);
                command.Parameters.AddWithValue("@description", userGroup.Description);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<UserGroup> GetUserGroupsForUser(int userId)
    {
        var userGroups = new List<UserGroup>();
        using (var connection = CreateConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(
                @"SELECT ug.* FROM user_groups ug
                      JOIN user_group_memberships ugm ON ug.group_id = ugm.group_id
                      WHERE ugm.user_id = @userId",
                connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userGroups.Add(new UserGroup
                        {
                            GroupId = Convert.ToInt32(reader["group_id"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString()
                        });
                    }
                }
            }
        }
        return userGroups;
    }
}