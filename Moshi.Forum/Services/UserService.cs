using Moshi.Forums.Data;
using Moshi.Forums.Models;

namespace Moshi.Forums.Services;

public class UserService
{
    private readonly UserRepository _repository;

    public UserService(UserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<int> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        // In a real application, you would hash the password here
        // user.PasswordHash = HashPassword(user.Password);
        return await _repository.CreateAsync(user);
    }

    public async Task<User> UpdateUserAsync(int id, User user)
    {
        var existingUser = await _repository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return null;
        }

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        // Update other fields as necessary
        // If updating password: existingUser.PasswordHash = HashPassword(user.NewPassword);

        await _repository.UpdateAsync(existingUser);
        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        await _repository.DeleteAsync(id);
        return true;
    }

    // private string HashPassword(string password)
    // {
    //     // Implement password hashing logic here
    // }
}
