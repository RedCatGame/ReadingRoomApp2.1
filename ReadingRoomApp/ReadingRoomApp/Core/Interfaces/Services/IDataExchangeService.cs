using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Services
{
    public interface IDataExchangeService
    {
        Task<string> ExportBooksAsync(IEnumerable<Book> books, string format = "json");
        Task<IEnumerable<Book>> ImportBooksAsync(string filePath);

        Task<string> ExportReadersAsync(IEnumerable<Reader> readers, string format = "json");
        Task<IEnumerable<Reader>> ImportReadersAsync(string filePath);

        Task<string> ExportAuthorsAsync(IEnumerable<Author> authors, string format = "json");
        Task<IEnumerable<Author>> ImportAuthorsAsync(string filePath);

        Task<string> ExportGenresAsync(IEnumerable<Genre> genres, string format = "json");
        Task<IEnumerable<Genre>> ImportGenresAsync(string filePath);

        Task BackupDatabaseAsync(string backupPath = null);
        Task RestoreDatabaseAsync(string backupPath);
    }
}