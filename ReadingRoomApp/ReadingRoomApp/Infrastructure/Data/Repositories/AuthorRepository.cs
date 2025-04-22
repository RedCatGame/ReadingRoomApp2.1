using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly string _connectionString;

        public AuthorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Author> AddAsync(Author entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<List<Author>> GetAllAsync()
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Author> UpdateAsync(Author entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }
    }
}