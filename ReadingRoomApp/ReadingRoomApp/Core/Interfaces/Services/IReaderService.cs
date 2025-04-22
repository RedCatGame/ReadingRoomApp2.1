using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Services
{
    public interface IReaderService
    {
        Task<List<Reader>> GetAllReadersAsync();
        Task<Reader> GetReaderByIdAsync(int id);
        Task<Reader> GetReaderByEmailAsync(string email);
        Task<Reader> AddReaderAsync(Reader reader);
        Task<Reader> UpdateReaderAsync(Reader reader);
        Task<bool> DeleteReaderAsync(int id);
        Task<bool> BorrowBookAsync(int readerId, int bookId);
        Task<bool> ReturnBookAsync(int readerId, int bookId);
    }
}