using Moshi.Blog.Data;
using Moshi.Blog.Models;

namespace Moshi.Blog.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<User> CreateUser(User user)
        {
            // Here you might want to add password hashing logic
            return await _userRepository.CreateUser(user);
        }

        public async Task<User> UpdateUser(int id, User user)
        {
            user.Id = id;
            return await _userRepository.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await _userRepository.DeleteUser(id);
        }
    }
}