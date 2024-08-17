using Moshi.SubtitlesSite.Data;
using Moshi.SubtitlesSite.Models;

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
}
