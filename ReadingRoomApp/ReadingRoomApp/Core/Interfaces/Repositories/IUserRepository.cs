using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> SetPasswordAsync(int userId, string passwordHash, string salt);
        Task<bool> CheckPasswordAsync(int userId, string passwordHash);
        Task<(string hash, string salt)> GetPasswordDataAsync(string username);
    }
}