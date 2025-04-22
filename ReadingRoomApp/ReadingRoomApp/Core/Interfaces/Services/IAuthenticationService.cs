using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}