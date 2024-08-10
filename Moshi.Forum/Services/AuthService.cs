using Microsoft.IdentityModel.Tokens;
using Moshi.Forums.Data;
using Moshi.Forums.Models;
using Moshi.Forums.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Moshi.Forums.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(string token, UserDto userDto)> LoginUser(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password");
            }

            await _userRepository.UpdateLastLoginAsync(user.Id);
            var token = GenerateJwtToken(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return (token, userDto);
        }

        public async Task<(string token, UserDto userDto)> RegisterUser(UserRegistrationDto registrationDto)
        {
            if (await _userRepository.IsUsernameTakenAsync(registrationDto.Username) ||
                await _userRepository.IsEmailTakenAsync(registrationDto.Email))
            {
                throw new Exception("Username or email already taken");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            var user = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            var token = GenerateJwtToken(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return (token, userDto);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
