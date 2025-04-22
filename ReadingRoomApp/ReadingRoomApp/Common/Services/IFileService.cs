using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadingRoomApp.Common.Services
{
    public interface IFileService
    {
        Task<string> ReadTextAsync(string filePath);
        Task WriteTextAsync(string filePath, string content);
        Task<bool> FileExistsAsync(string filePath);
        Task<IEnumerable<string>> GetFilesAsync(string directoryPath, string searchPattern = "*.*");
        Task<string> SelectFileAsync(string title, string filter);
        Task<string> SaveFileAsync(string title, string defaultFileName, string filter);
    }
}