using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;
using System;
using System.Collections.Generic;

namespace Moshi.SubtitlesSite.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User GetUserById(int userId)
    {
        return _userRepository.GetUserById(userId);
    }

    public User GetUserByUsername(string username)
    {
        return _userRepository.GetUserByUsername(username);
    }

    public bool UpdateUser(User user)
    {
        return _userRepository.UpdateUser(user);
    }

    public IEnumerable<UserBadge> GetUserBadges(int userId)
    {
        return _userRepository.GetUserBadges(userId);
    }

    public bool AddUserBadge(UserBadge badge)
    {
        return _userRepository.AddUserBadge(badge);
    }

    public string GetUserCountry(int userId)
    {
        var user = GetUserById(userId);
        return user?.Country;
    }

    public IEnumerable<ProfileComment> GetUserProfileComments(int userId)
    {
        return _userRepository.GetUserProfileComments(userId);
    }

    public bool AddProfileComment(int userId, string commenterUsername, string comment)
    {
        var commenter = GetUserByUsername(commenterUsername);
        if (commenter == null)
        {
            return false;
        }

        var profileComment = new ProfileComment
        {
            UserId = userId,
            CommenterId = commenter.UserId,
            CommentText = comment,
            CommentDate = DateTime.UtcNow
        };

        return _userRepository.AddProfileComment(profileComment);
    }

    public int GetUserUploadedSubtitlesCount(int userId)
    {
        return _userRepository.GetUserUploadedSubtitlesCount(userId);
    }

    public bool UpdateLastLoginDate(int userId)
    {
        var user = GetUserById(userId);
        if (user == null)
        {
            return false;
        }

        user.LastLoginDate = DateTime.UtcNow;
        return UpdateUser(user);
    }
}
