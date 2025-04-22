using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Helpers;
using ReadingRoomApp.Common.Constants;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class FileAuthorRepository : IAuthorRepository
    {
        private List<Author> _authors;

        public FileAuthorRepository()
        {
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            _authors = JsonHelper.LoadFromJsonFile<Author>(AppConstants.AUTHORS_FILE_PATH);
        }

        private void SaveAuthors()
        {
            JsonHelper.SaveToJsonFile(_authors, AppConstants.AUTHORS_FILE_PATH);
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await Task.FromResult(_authors);
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await Task.FromResult(_authors.FirstOrDefault(a => a.Id == id));
        }

        public async Task<Author> AddAsync(Author author)
        {
            author.Id = _authors.Any() ? _authors.Max(a => a.Id) + 1 : 1;
            _authors.Add(author);
            SaveAuthors();
            return await Task.FromResult(author);
        }

        public async Task<Author> UpdateAsync(Author author)
        {
            var existingAuthor = _authors.FirstOrDefault(a => a.Id == author.Id);
            if (existingAuthor == null)
                return null;

            int index = _authors.IndexOf(existingAuthor);
            _authors[index] = author;
            SaveAuthors();
            return await Task.FromResult(author);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = _authors.FirstOrDefault(a => a.Id == id);
            if (author == null)
                return false;

            _authors.Remove(author);
            SaveAuthors();
            return await Task.FromResult(true);
        }
    }
}