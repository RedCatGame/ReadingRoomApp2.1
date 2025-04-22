using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReadingRoomApp.Common.Constants;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;

namespace ReadingRoomApp.Core.Services
{
    public class DataExchangeService : IDataExchangeService
    {
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private readonly string _exportDirectory;
        private readonly string _backupDirectory;

        public DataExchangeService(
            IBookService bookService,
            IReaderService readerService,
            IAuthorService authorService,
            IGenreService genreService)
        {
            _bookService = bookService ?? App.BookService;
            _readerService = readerService ?? App.ReaderService;
            _authorService = authorService ?? App.AuthorService;
            _genreService = genreService ?? App.GenreService;

            _exportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export");
            _backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");

            if (!Directory.Exists(_exportDirectory))
            {
                Directory.CreateDirectory(_exportDirectory);
            }

            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        public async Task<string> ExportBooksAsync(IEnumerable<Book> books, string format = "json")
        {
            App.Logger.LogInfo($"Экспорт книг в формате {format}");

            string fileName = $"Books_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_exportDirectory, fileName);

            if (format.ToLower() == "json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(books, options);
                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            else if (format.ToLower() == "csv")
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Заголовки CSV
                    await writer.WriteLineAsync("Id,Title,ISBN,Description,PublicationYear,AuthorId,GenreId,IsAvailable");

                    // Данные
                    foreach (var book in books)
                    {
                        await writer.WriteLineAsync($"{book.Id},\"{book.Title}\",\"{book.ISBN}\",\"{book.Description?.Replace("\"", "\"\"")}\",{book.PublicationYear},{book.Author?.Id ?? 0},{book.Genre?.Id ?? 0},{book.IsAvailable}");
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {format} не поддерживается");
            }

            App.Logger.LogInfo($"Книги экспортированы в файл: {filePath}");
            return filePath;
        }

        public async Task<IEnumerable<Book>> ImportBooksAsync(string filePath)
        {
            App.Logger.LogInfo($"Импорт книг из файла: {filePath}");

            string fileExtension = Path.GetExtension(filePath).ToLower();
            List<Book> importedBooks;

            if (fileExtension == ".json")
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                importedBooks = JsonSerializer.Deserialize<List<Book>>(jsonContent);
            }
            else if (fileExtension == ".csv")
            {
                importedBooks = new List<Book>();
                using (var reader = new StreamReader(filePath))
                {
                    // Пропускаем заголовки
                    await reader.ReadLineAsync();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(',');

                        if (values.Length >= 8)
                        {
                            var book = new Book
                            {
                                Id = int.Parse(values[0]),
                                Title = values[1].Trim('"'),
                                ISBN = values[2].Trim('"'),
                                Description = values[3].Trim('"'),
                                PublicationYear = int.Parse(values[4]),
                                Author = values[5] != "0" ? await _authorService.GetByIdAsync(int.Parse(values[5])) : null,
                                Genre = values[6] != "0" ? await _genreService.GetByIdAsync(int.Parse(values[6])) : null,
                                IsAvailable = bool.Parse(values[7])
                            };

                            importedBooks.Add(book);
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {fileExtension} не поддерживается");
            }

            // Сохраняем импортированные книги
            foreach (var book in importedBooks)
            {
                if (await _bookService.GetByIdAsync(book.Id) != null)
                {
                    await _bookService.UpdateAsync(book);
                }
                else
                {
                    await _bookService.AddAsync(book);
                }
            }

            App.Logger.LogInfo($"Импортировано {importedBooks.Count} книг");
            return importedBooks;
        }

        public async Task<string> ExportReadersAsync(IEnumerable<Reader> readers, string format = "json")
        {
            App.Logger.LogInfo($"Экспорт читателей в формате {format}");

            string fileName = $"Readers_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_exportDirectory, fileName);

            if (format.ToLower() == "json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(readers, options);
                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            else if (format.ToLower() == "csv")
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Заголовки CSV
                    await writer.WriteLineAsync("Id,FirstName,LastName,Email,PhoneNumber,RegistrationDate");

                    // Данные
                    foreach (var reader in readers)
                    {
                        await writer.WriteLineAsync($"{reader.Id},\"{reader.FirstName}\",\"{reader.LastName}\",\"{reader.Email}\",\"{reader.PhoneNumber}\",{reader.RegistrationDate:yyyy-MM-dd}");
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {format} не поддерживается");
            }

            App.Logger.LogInfo($"Читатели экспортированы в файл: {filePath}");
            return filePath;
        }

        public async Task<IEnumerable<Reader>> ImportReadersAsync(string filePath)
        {
            App.Logger.LogInfo($"Импорт читателей из файла: {filePath}");

            string fileExtension = Path.GetExtension(filePath).ToLower();
            List<Reader> importedReaders;

            if (fileExtension == ".json")
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                importedReaders = JsonSerializer.Deserialize<List<Reader>>(jsonContent);
            }
            else if (fileExtension == ".csv")
            {
                importedReaders = new List<Reader>();
                using (var reader = new StreamReader(filePath))
                {
                    // Пропускаем заголовки
                    await reader.ReadLineAsync();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(',');

                        if (values.Length >= 6)
                        {
                            var readerEntity = new Reader
                            {
                                Id = int.Parse(values[0]),
                                FirstName = values[1].Trim('"'),
                                LastName = values[2].Trim('"'),
                                Email = values[3].Trim('"'),
                                PhoneNumber = values[4].Trim('"'),
                                RegistrationDate = DateTime.Parse(values[5])
                            };

                            importedReaders.Add(readerEntity);
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {fileExtension} не поддерживается");
            }

            // Сохраняем импортированных читателей
            foreach (var reader in importedReaders)
            {
                if (await _readerService.GetByIdAsync(reader.Id) != null)
                {
                    await _readerService.UpdateAsync(reader);
                }
                else
                {
                    await _readerService.AddAsync(reader);
                }
            }

            App.Logger.LogInfo($"Импортировано {importedReaders.Count} читателей");
            return importedReaders;
        }

        public async Task<string> ExportAuthorsAsync(IEnumerable<Author> authors, string format = "json")
        {
            App.Logger.LogInfo($"Экспорт авторов в формате {format}");

            string fileName = $"Authors_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_exportDirectory, fileName);

            if (format.ToLower() == "json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(authors, options);
                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            else if (format.ToLower() == "csv")
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Заголовки CSV
                    await writer.WriteLineAsync("Id,FirstName,LastName,BirthDate,Biography");

                    // Данные
                    foreach (var author in authors)
                    {
                        await writer.WriteLineAsync($"{author.Id},\"{author.FirstName}\",\"{author.LastName}\",{author.BirthDate:yyyy-MM-dd},\"{author.Biography?.Replace("\"", "\"\"")}\"");
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {format} не поддерживается");
            }

            App.Logger.LogInfo($"Авторы экспортированы в файл: {filePath}");
            return filePath;
        }

        public async Task<IEnumerable<Author>> ImportAuthorsAsync(string filePath)
        {
            App.Logger.LogInfo($"Импорт авторов из файла: {filePath}");

            string fileExtension = Path.GetExtension(filePath).ToLower();
            List<Author> importedAuthors;

            if (fileExtension == ".json")
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                importedAuthors = JsonSerializer.Deserialize<List<Author>>(jsonContent);
            }
            else if (fileExtension == ".csv")
            {
                importedAuthors = new List<Author>();
                using (var reader = new StreamReader(filePath))
                {
                    // Пропускаем заголовки
                    await reader.ReadLineAsync();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(',');

                        if (values.Length >= 5)
                        {
                            var author = new Author
                            {
                                Id = int.Parse(values[0]),
                                FirstName = values[1].Trim('"'),
                                LastName = values[2].Trim('"'),
                                BirthDate = DateTime.Parse(values[3]),
                                Biography = values[4].Trim('"')
                            };

                            importedAuthors.Add(author);
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {fileExtension} не поддерживается");
            }

            // Сохраняем импортированных авторов
            foreach (var author in importedAuthors)
            {
                if (await _authorService.GetByIdAsync(author.Id) != null)
                {
                    await _authorService.UpdateAsync(author);
                }
                else
                {
                    await _authorService.AddAsync(author);
                }
            }

            App.Logger.LogInfo($"Импортировано {importedAuthors.Count} авторов");
            return importedAuthors;
        }

        public async Task<string> ExportGenresAsync(IEnumerable<Genre> genres, string format = "json")
        {
            App.Logger.LogInfo($"Экспорт жанров в формате {format}");

            string fileName = $"Genres_Export_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
            string filePath = Path.Combine(_exportDirectory, fileName);

            if (format.ToLower() == "json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(genres, options);
                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            else if (format.ToLower() == "csv")
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Заголовки CSV
                    await writer.WriteLineAsync("Id,Name,Description");

                    // Данные
                    foreach (var genre in genres)
                    {
                        await writer.WriteLineAsync($"{genre.Id},\"{genre.Name}\",\"{genre.Description?.Replace("\"", "\"\"")}\"");
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {format} не поддерживается");
            }

            App.Logger.LogInfo($"Жанры экспортированы в файл: {filePath}");
            return filePath;
        }

        public async Task<IEnumerable<Genre>> ImportGenresAsync(string filePath)
        {
            App.Logger.LogInfo($"Импорт жанров из файла: {filePath}");

            string fileExtension = Path.GetExtension(filePath).ToLower();
            List<Genre> importedGenres;

            if (fileExtension == ".json")
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                importedGenres = JsonSerializer.Deserialize<List<Genre>>(jsonContent);
            }
            else if (fileExtension == ".csv")
            {
                importedGenres = new List<Genre>();
                using (var reader = new StreamReader(filePath))
                {
                    // Пропускаем заголовки
                    await reader.ReadLineAsync();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(',');

                        if (values.Length >= 3)
                        {
                            var genre = new Genre
                            {
                                Id = int.Parse(values[0]),
                                Name = values[1].Trim('"'),
                                Description = values[2].Trim('"')
                            };

                            importedGenres.Add(genre);
                        }
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"Формат {fileExtension} не поддерживается");
            }

            // Сохраняем импортированные жанры
            foreach (var genre in importedGenres)
            {
                if (await _genreService.GetByIdAsync(genre.Id) != null)
                {
                    await _genreService.UpdateAsync(genre);
                }
                else
                {
                    await _genreService.AddAsync(genre);
                }
            }

            App.Logger.LogInfo($"Импортировано {importedGenres.Count} жанров");
            return importedGenres;
        }

        public async Task BackupDatabaseAsync(string backupPath = null)
        {
            App.Logger.LogInfo("Создание резервной копии базы данных");

            if (string.IsNullOrEmpty(backupPath))
            {
                backupPath = Path.Combine(_backupDirectory, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}");

                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }
            }

            var books = await _bookService.GetAllAsync();
            var readers = await _readerService.GetAllAsync();
            var authors = await _authorService.GetAllAsync();
            var genres = await _genreService.GetAllAsync();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            await File.WriteAllTextAsync(Path.Combine(backupPath, "books.json"), JsonSerializer.Serialize(books, options));
            await File.WriteAllTextAsync(Path.Combine(backupPath, "readers.json"), JsonSerializer.Serialize(readers, options));
            await File.WriteAllTextAsync(Path.Combine(backupPath, "authors.json"), JsonSerializer.Serialize(authors, options));
            await File.WriteAllTextAsync(Path.Combine(backupPath, "genres.json"), JsonSerializer.Serialize(genres, options));

            App.Logger.LogInfo($"Резервная копия создана в директории: {backupPath}");
        }

        public async Task RestoreDatabaseAsync(string backupPath)
        {
            App.Logger.LogInfo($"Восстановление данных из резервной копии: {backupPath}");

            if (!Directory.Exists(backupPath))
            {
                throw new DirectoryNotFoundException($"Директория резервной копии не найдена: {backupPath}");
            }

            string booksPath = Path.Combine(backupPath, "books.json");
            string readersPath = Path.Combine(backupPath, "readers.json");
            string authorsPath = Path.Combine(backupPath, "authors.json");
            string genresPath = Path.Combine(backupPath, "genres.json");

            if (File.Exists(genresPath))
            {
                await ImportGenresAsync(genresPath);
            }

            if (File.Exists(authorsPath))
            {
                await ImportAuthorsAsync(authorsPath);
            }

            if (File.Exists(readersPath))
            {
                await ImportReadersAsync(readersPath);
            }

            if (File.Exists(booksPath))
            {
                await ImportBooksAsync(booksPath);
            }

            App.Logger.LogInfo("Восстановление данных из резервной копии завершено");
        }
    }
}