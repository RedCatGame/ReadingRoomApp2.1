using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Helpers;
using ReadingRoomApp.Common.Constants;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class FileUserRepository : IUserRepository
    {
        private class UserWithPassword : User
        {
            public string PasswordHash { get; set; }
            public string Salt { get; set; }
        }

        private List<UserWithPassword> _users;

        public FileUserRepository()
        {
            LoadUsers();
            EnsureAdminExists();
        }

        private void LoadUsers()
        {
            _users = JsonHelper.LoadFromJsonFile<UserWithPassword>(AppConstants.USERS_FILE_PATH);
        }

        private void SaveUsers()
        {
            JsonHelper.SaveToJsonFile(_users, AppConstants.USERS_FILE_PATH);
        }

        private void EnsureAdminExists()
        {
            // Проверяем, есть ли пользователь с ролью Admin
            if (!_users.Any(u => u.Role == Core.Domain.Enums.UserRole.Admin))
            {
                // Создаем администратора по умолчанию
                var salt = PasswordHasher.GenerateSalt("admin");
                var adminUser = new UserWithPassword
                {
                    Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1,
                    Username = "admin",
                    Email = "admin@readingroom.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = Core.Domain.Enums.UserRole.Admin,
                    PasswordHash = PasswordHasher.HashPassword("admin", salt),
                    Salt = salt
                };

                _users.Add(adminUser);
                SaveUsers();
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await Task.FromResult<List<User>>(_users.ToList());
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await Task.FromResult<User>(_users.FirstOrDefault(u => u.Id == id));
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await Task.FromResult<User>(_users.FirstOrDefault(u => u.Username == username));
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await Task.FromResult<User>(_users.FirstOrDefault(u => u.Email == email));
        }

        public async Task<User> AddAsync(User user)
        {
            var userWithPassword = new UserWithPassword
            {
                Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                // Пароль должен быть установлен отдельно
                PasswordHash = "",
                Salt = ""
            };

            _users.Add(userWithPassword);
            SaveUsers();
            user.Id = userWithPassword.Id;
            return await Task.FromResult(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
                return null;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Role = user.Role;

            SaveUsers();
            return await Task.FromResult(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return false;

            _users.Remove(user);
            SaveUsers();
            return await Task.FromResult(true);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return await Task.FromResult(!_users.Any(u => u.Username == username));
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return await Task.FromResult(!_users.Any(u => u.Email == email));
        }

        public async Task<bool> SetPasswordAsync(int userId, string passwordHash, string salt)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;

            user.PasswordHash = passwordHash;
            user.Salt = salt;
            SaveUsers();
            return await Task.FromResult(true);
        }

        public async Task<bool> CheckPasswordAsync(int userId, string passwordHash)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;

            return await Task.FromResult(user.PasswordHash == passwordHash);
        }

        public async Task<(string hash, string salt)> GetPasswordDataAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return (null, null);

            return await Task.FromResult((user.PasswordHash, user.Salt));
        }
    }
}