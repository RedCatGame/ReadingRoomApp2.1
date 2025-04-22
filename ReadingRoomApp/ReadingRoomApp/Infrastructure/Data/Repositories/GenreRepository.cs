using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string _connectionString;

        public GenreRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Genre> AddAsync(Genre entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Genre> UpdateAsync(Genre entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }
    }
}