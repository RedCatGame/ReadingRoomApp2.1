using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Core.Interfaces.Services;

namespace ReadingRoomApp.Core.Services
{
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IBookRepository _bookRepository;

        public ReaderService(IReaderRepository readerRepository, IBookRepository bookRepository)
        {
            _readerRepository = readerRepository;
            _bookRepository = bookRepository;
        }

        public async Task<List<Reader>> GetAllReadersAsync()
        {
            return await _readerRepository.GetAllAsync();
        }

        public async Task<Reader> GetReaderByIdAsync(int id)
        {
            return await _readerRepository.GetByIdAsync(id);
        }

        public async Task<Reader> GetReaderByEmailAsync(string email)
        {
            return await _readerRepository.GetReaderByEmailAsync(email);
        }

        public async Task<Reader> AddReaderAsync(Reader reader)
        {
            return await _readerRepository.AddAsync(reader);
        }

        public async Task<Reader> UpdateReaderAsync(Reader reader)
        {
            return await _readerRepository.UpdateAsync(reader);
        }

        public async Task<bool> DeleteReaderAsync(int id)
        {
            return await _readerRepository.DeleteAsync(id);
        }

        public async Task<bool> BorrowBookAsync(int readerId, int bookId)
        {
            // Реализация будет дополнена в будущем
            return true;
        }

        public async Task<bool> ReturnBookAsync(int readerId, int bookId)
        {
            // Реализация будет дополнена в будущем
            return true;
        }
    }
}