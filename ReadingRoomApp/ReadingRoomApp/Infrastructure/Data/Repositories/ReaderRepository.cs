using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly string _connectionString;

        public ReaderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Reader> AddAsync(Reader entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<List<Reader>> GetAllAsync()
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Reader> GetByIdAsync(int id)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Reader> GetReaderByEmailAsync(string email)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }

        public async Task<Reader> UpdateAsync(Reader entity)
        {
            throw new NotImplementedException("Database implementation not yet available");
        }
    }
}