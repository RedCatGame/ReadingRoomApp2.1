using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<List<Book>> GetBooksByAuthorAsync(int authorId);
        Task<List<Book>> GetBooksByGenreAsync(int genreId);
    }
}