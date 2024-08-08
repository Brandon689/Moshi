using Moshi.Forums.Data;
using Moshi.Forums.Models;
using Moshi.Forums.Models.DTOs;

namespace Moshi.Forums.Services;

public class ProfileService
{
    private readonly UserRepository _userRepository;

    public ProfileService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> GetUserProfile(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Map user to UserProfileDto
        return MapUserToProfileDto(user);
    }

    public async Task UpdateUserProfile(int userId, UserProfileUpdateDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Update user properties
        user.Username = updateDto.Username ?? user.Username;
        user.Email = updateDto.Email ?? user.Email;

        await _userRepository.UpdateAsync(user);
    }

    private UserProfileDto MapUserToProfileDto(User user)
    {
        return new UserProfileDto();
        // Implement mapping logic
    }
}
