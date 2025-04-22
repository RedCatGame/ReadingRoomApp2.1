using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Infrastructure.Helpers;

namespace ReadingRoomApp.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            var (storedHash, salt) = await _userRepository.GetPasswordDataAsync(username);
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(salt))
                return null;

            string hash = PasswordHasher.HashPassword(password, salt);
            if (hash != storedHash)
                return null;

            return user;
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            if (!await _userRepository.IsUsernameUniqueAsync(user.Username) ||
                !await _userRepository.IsEmailUniqueAsync(user.Email))
            {
                return false;
            }

            // Добавление пользователя
            var newUser = await _userRepository.AddAsync(user);
            if (newUser == null)
                return false;

            // Установка пароля
            string salt = PasswordHasher.GenerateSalt(user.Username);
            string hash = PasswordHasher.HashPassword(password, salt);

            return await _userRepository.SetPasswordAsync(newUser.Id, hash, salt);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var (storedHash, salt) = await _userRepository.GetPasswordDataAsync(user.Username);
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(salt))
                return false;

            // Проверка старого пароля
            string oldHash = PasswordHasher.HashPassword(oldPassword, salt);
            if (oldHash != storedHash)
                return false;

            // Установка нового пароля
            string newHash = PasswordHasher.HashPassword(newPassword, salt);
            return await _userRepository.SetPasswordAsync(userId, newHash, salt);
        }
    }
}