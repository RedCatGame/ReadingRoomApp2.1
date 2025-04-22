using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Repositories
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<Reader> GetReaderByEmailAsync(string email);
    }
}