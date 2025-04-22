using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Helpers;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class FileBookRepository : IBookRepository
    {
        private const string FILE_PATH = @"Data\books.json";
        private List<Book> _books;

        public FileBookRepository()
        {
            LoadBooks();
        }

        private void LoadBooks()
        {
            _books = JsonHelper.LoadFromJsonFile<Book>(FILE_PATH);
        }

        private void SaveBooks()
        {
            JsonHelper.SaveToJsonFile(_books, FILE_PATH);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await Task.FromResult(_books);
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
        }

        public async Task<Book> AddAsync(Book book)
        {
            book.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(book);
            SaveBooks();
            return await Task.FromResult(book);
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null)
                return null;

            int index = _books.IndexOf(existingBook);
            _books[index] = book;
            SaveBooks();
            return await Task.FromResult(book);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return false;

            _books.Remove(book);
            SaveBooks();
            return await Task.FromResult(true);
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await Task.FromResult(_books.Where(b => b.Author?.Id == authorId).ToList());
        }

        public async Task<List<Book>> GetBooksByGenreAsync(int genreId)
        {
            return await Task.FromResult(_books.Where(b => b.Genre?.Id == genreId).ToList());
        }
    }
}