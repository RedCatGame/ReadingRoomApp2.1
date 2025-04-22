using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Core.Interfaces.Services
{
    public interface IReportService
    {
        Task<string> GenerateBookListReportAsync(IEnumerable<Book> books, string format = "pdf");
        Task<string> GenerateReaderListReportAsync(IEnumerable<Reader> readers, string format = "pdf");
        Task<string> GenerateBorrowingReportAsync(int readerId, string format = "pdf");
        Task<string> GenerateAuthorReportAsync(int authorId, string format = "pdf");
        Task<string> GenerateGenreStatisticsReportAsync(string format = "pdf");
        Task<string> GenerateActivityReportAsync(System.DateTime startDate, System.DateTime endDate, string format = "pdf");
    }
}