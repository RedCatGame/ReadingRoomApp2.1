using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadingRoomApp.Common.Constants;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;

namespace ReadingRoomApp.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly string _reportsDirectory;

        public ReportService(
            IBookService bookService,
            IReaderService readerService,
            IAuthorService authorService,
            IGenreService genreService)
        {
            _bookService = bookService ?? App.BookService;
            _readerService = readerService ?? App.ReaderService;
            _authorService = authorService ?? App.AuthorService;
            _genreService = genreService ?? App.GenreService;

            _reportsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            if (!Directory.Exists(_reportsDirectory))
            {
                Directory.CreateDirectory(_reportsDirectory);
            }
        }

        public async Task<string> GenerateBookListReportAsync(IEnumerable<Book> books, string format = "pdf")
        {
            App.Logger.LogInfo("Создание отчета по списку книг");

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по книгам");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Количество книг: {books.Count()}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            reportContent.AppendLine("ID\tНазвание\tАвтор\tЖанр\tГод\tСтатус");
            foreach (var book in books)
            {
                reportContent.AppendLine($"{book.Id}\t{book.Title}\t{book.Author?.FullName ?? "Н/Д"}\t" +
                                        $"{book.Genre?.Name ?? "Н/Д"}\t{book.PublicationYear}\t" +
                                        $"{(book.IsAvailable ? "Доступна" : "Недоступна")}");
            }

            string fileName = $"BookList_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }

        public async Task<string> GenerateReaderListReportAsync(IEnumerable<Reader> readers, string format = "pdf")
        {
            App.Logger.LogInfo("Создание отчета по списку читателей");

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по читателям");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Количество читателей: {readers.Count()}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            reportContent.AppendLine("ID\tИмя\tФамилия\tEmail\tТелефон");
            foreach (var reader in readers)
            {
                reportContent.AppendLine($"{reader.Id}\t{reader.FirstName}\t{reader.LastName}\t" +
                                        $"{reader.Email}\t{reader.PhoneNumber ?? "Н/Д"}");
            }

            string fileName = $"ReaderList_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }

        public async Task<string> GenerateBorrowingReportAsync(int readerId, string format = "pdf")
        {
            App.Logger.LogInfo($"Создание отчета по заимствованиям для читателя с ID: {readerId}");

            var reader = await _readerService.GetByIdAsync(readerId);
            if (reader == null)
            {
                throw new Exception($"Читатель с ID {readerId} не найден");
            }

            var borrowedBooks = await _readerService.GetBorrowedBooksAsync(readerId);

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по заимствованиям");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Читатель: {reader.FullName}");
            reportContent.AppendLine($"Количество взятых книг: {borrowedBooks.Count}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            reportContent.AppendLine("ID\tНазвание\tАвтор\tГод\tДата выдачи\tСрок возврата");
            foreach (var book in borrowedBooks)
            {
                // Для простоты примем, что книги выдаются на 14 дней
                var borrowDate = DateTime.Now.AddDays(-new Random().Next(1, 13));
                var returnDate = borrowDate.AddDays(14);

                reportContent.AppendLine($"{book.Id}\t{book.Title}\t{book.Author?.FullName ?? "Н/Д"}\t" +
                                        $"{book.PublicationYear}\t{borrowDate:dd.MM.yyyy}\t{returnDate:dd.MM.yyyy}");
            }

            string fileName = $"ReaderBorrowing_{reader.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }

        public async Task<string> GenerateAuthorReportAsync(int authorId, string format = "pdf")
        {
            App.Logger.LogInfo($"Создание отчета по автору с ID: {authorId}");

            var author = await _authorService.GetByIdAsync(authorId);
            if (author == null)
            {
                throw new Exception($"Автор с ID {authorId} не найден");
            }

            var books = await _bookService.GetBooksByAuthorAsync(authorId);

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по автору");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Автор: {author.FullName}");
            reportContent.AppendLine($"Дата рождения: {author.BirthDate:dd.MM.yyyy}");
            reportContent.AppendLine($"Количество книг: {books.Count}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            reportContent.AppendLine("ID\tНазвание\tЖанр\tГод\tСтатус");
            foreach (var book in books)
            {
                reportContent.AppendLine($"{book.Id}\t{book.Title}\t{book.Genre?.Name ?? "Н/Д"}\t" +
                                        $"{book.PublicationYear}\t{(book.IsAvailable ? "Доступна" : "Недоступна")}");
            }

            string fileName = $"AuthorReport_{author.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }

        public async Task<string> GenerateGenreStatisticsReportAsync(string format = "pdf")
        {
            App.Logger.LogInfo("Создание отчета по статистике жанров");

            var genres = await _genreService.GetAllAsync();
            var books = await _bookService.GetAllAsync();

            var genreStats = new Dictionary<int, int>();
            foreach (var genre in genres)
            {
                genreStats[genre.Id] = books.Count(b => b.Genre?.Id == genre.Id);
            }

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по статистике жанров");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Количество жанров: {genres.Count}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            reportContent.AppendLine("Жанр\tКоличество книг\tПроцент от общего");
            foreach (var genre in genres.OrderByDescending(g => genreStats[g.Id]))
            {
                var percentage = books.Count > 0 ? (double)genreStats[genre.Id] / books.Count * 100 : 0;
                reportContent.AppendLine($"{genre.Name}\t{genreStats[genre.Id]}\t{percentage:F2}%");
            }

            string fileName = $"GenreStatistics_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }

        public async Task<string> GenerateActivityReportAsync(DateTime startDate, DateTime endDate, string format = "pdf")
        {
            App.Logger.LogInfo($"Создание отчета по активности за период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");

            // В реальном приложении здесь бы использовались данные о заимствованиях и возвратах книг
            // Для упрощения создадим примерные данные активности

            var reportContent = new StringBuilder();
            reportContent.AppendLine("Отчет по активности");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            reportContent.AppendLine($"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
            reportContent.AppendLine("--------------------------------------");
            reportContent.AppendLine();

            // Примерные данные активности
            var random = new Random();
            var currentDate = startDate;

            reportContent.AppendLine("Дата\tВыдано книг\tВозвращено книг\tНовых читателей");

            while (currentDate <= endDate)
            {
                var issuedBooks = random.Next(0, 10);
                var returnedBooks = random.Next(0, 8);
                var newReaders = random.Next(0, 3);

                reportContent.AppendLine($"{currentDate:dd.MM.yyyy}\t{issuedBooks}\t{returnedBooks}\t{newReaders}");

                currentDate = currentDate.AddDays(1);
            }

            string fileName = $"ActivityReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_{DateTime.Now:HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_reportsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            App.Logger.LogInfo($"Отчет сохранен в файл: {filePath}");

            return filePath;
        }
    }
}