using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Helpers;
using ReadingRoomApp.Common.Constants;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class FileReaderRepository : IReaderRepository
    {
        private List<Reader> _readers;

        public FileReaderRepository()
        {
            LoadReaders();
        }

        private void LoadReaders()
        {
            _readers = JsonHelper.LoadFromJsonFile<Reader>(AppConstants.READERS_FILE_PATH);
        }

        private void SaveReaders()
        {
            JsonHelper.SaveToJsonFile(_readers, AppConstants.READERS_FILE_PATH);
        }

        public async Task<List<Reader>> GetAllAsync()
        {
            return await Task.FromResult(_readers);
        }

        public async Task<Reader> GetByIdAsync(int id)
        {
            return await Task.FromResult(_readers.FirstOrDefault(r => r.Id == id));
        }

        public async Task<Reader> GetReaderByEmailAsync(string email)
        {
            return await Task.FromResult(_readers.FirstOrDefault(r => r.Email == email));
        }

        public async Task<Reader> AddAsync(Reader reader)
        {
            reader.Id = _readers.Any() ? _readers.Max(r => r.Id) + 1 : 1;
            _readers.Add(reader);
            SaveReaders();
            return await Task.FromResult(reader);
        }

        public async Task<Reader> UpdateAsync(Reader reader)
        {
            var existingReader = _readers.FirstOrDefault(r => r.Id == reader.Id);
            if (existingReader == null)
                return null;

            int index = _readers.IndexOf(existingReader);
            _readers[index] = reader;
            SaveReaders();
            return await Task.FromResult(reader);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reader = _readers.FirstOrDefault(r => r.Id == id);
            if (reader == null)
                return false;

            _readers.Remove(reader);
            SaveReaders();
            return await Task.FromResult(true);
        }
    }
}