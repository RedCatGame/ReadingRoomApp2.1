using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> AddAsync(User entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> CheckPasswordAsync(int userId, string passwordHash)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<User> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<(string hash, string salt)> GetPasswordDataAsync(string username)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> SetPasswordAsync(int userId, string passwordHash, string salt)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<User> UpdateAsync(User entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }
    }
}