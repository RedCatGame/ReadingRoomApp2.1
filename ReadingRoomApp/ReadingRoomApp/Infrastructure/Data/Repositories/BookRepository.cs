using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Infrastructure.Data;

namespace ReadingRoomApp.Infrastructure.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTablesExist();
        }

        private void EnsureTablesExist()
        {
            try
            {
                using (var context = new DatabaseContext(_connectionString))
                {
                    // Проверяем существование таблицы Books
                    var checkTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Books')
                        BEGIN
                            CREATE TABLE Books (
                                BookId INT PRIMARY KEY IDENTITY(1,1),
                                Title NVARCHAR(255) NOT NULL,
                                AuthorId INT NOT NULL,
                                GenreId INT NOT NULL,
                                PublicationYear INT NOT NULL,
                                IsAvailable BIT NOT NULL DEFAULT 1,
                                CreatedAt DATETIME DEFAULT GETDATE(),
                                UpdatedAt DATETIME DEFAULT GETDATE(),
                                CreatedByUserId INT
                            )
                        END";

                    using (var command = new SqlCommand(checkTableQuery, context.Connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring books table: {ex.Message}");
            }
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var books = new List<Book>();

            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    SELECT b.BookId, b.Title, b.PublicationYear, b.IsAvailable, 
                           a.AuthorId, a.FirstName, a.LastName, a.BirthDate, 
                           g.GenreId, g.Name, g.Description
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.AuthorId
                    LEFT JOIN Genres g ON b.GenreId = g.GenreId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                PublicationYear = reader.GetInt32(2),
                                IsAvailable = reader.GetBoolean(3),
                                Author = new Author
                                {
                                    Id = reader.GetInt32(4),
                                    FirstName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    LastName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                    BirthDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7)
                                },
                                Genre = new Genre
                                {
                                    Id = reader.GetInt32(8),
                                    Name = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                    Description = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                }
                            });
                        }
                    }
                }
            }

            return books;
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    SELECT b.BookId, b.Title, b.PublicationYear, b.IsAvailable, 
                           a.AuthorId, a.FirstName, a.LastName, a.BirthDate, 
                           g.GenreId, g.Name, g.Description
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.AuthorId
                    LEFT JOIN Genres g ON b.GenreId = g.GenreId
                    WHERE b.BookId = @BookId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@BookId", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                PublicationYear = reader.GetInt32(2),
                                IsAvailable = reader.GetBoolean(3),
                                Author = new Author
                                {
                                    Id = reader.GetInt32(4),
                                    FirstName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    LastName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                    BirthDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7)
                                },
                                Genre = new Genre
                                {
                                    Id = reader.GetInt32(8),
                                    Name = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                    Description = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                }
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Book> AddAsync(Book book)
        {
            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    INSERT INTO Books (Title, AuthorId, GenreId, PublicationYear, IsAvailable, CreatedByUserId)
                    VALUES (@Title, @AuthorId, @GenreId, @PublicationYear, @IsAvailable, @CreatedByUserId);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@AuthorId", book.Author.Id);
                    command.Parameters.AddWithValue("@GenreId", book.Genre.Id);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);
                    command.Parameters.AddWithValue("@CreatedByUserId", DBNull.Value); // По умолчанию NULL

                    // Получаем ID новой записи
                    var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    book.Id = newId;

                    return book;
                }
            }
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    UPDATE Books 
                    SET Title = @Title, 
                        AuthorId = @AuthorId, 
                        GenreId = @GenreId, 
                        PublicationYear = @PublicationYear, 
                        IsAvailable = @IsAvailable,
                        UpdatedAt = GETDATE()
                    WHERE BookId = @BookId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@BookId", book.Id);
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@AuthorId", book.Author.Id);
                    command.Parameters.AddWithValue("@GenreId", book.Genre.Id);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@IsAvailable", book.IsAvailable);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                        return book;
                }
            }

            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var context = new DatabaseContext(_connectionString))
            {
                var query = "DELETE FROM Books WHERE BookId = @BookId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@BookId", id);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<Book>> GetBooksByAuthorAsync(int authorId)
        {
            var books = new List<Book>();

            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    SELECT b.BookId, b.Title, b.PublicationYear, b.IsAvailable, 
                           a.AuthorId, a.FirstName, a.LastName, a.BirthDate, 
                           g.GenreId, g.Name, g.Description
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.AuthorId
                    LEFT JOIN Genres g ON b.GenreId = g.GenreId
                    WHERE b.AuthorId = @AuthorId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@AuthorId", authorId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                PublicationYear = reader.GetInt32(2),
                                IsAvailable = reader.GetBoolean(3),
                                Author = new Author
                                {
                                    Id = reader.GetInt32(4),
                                    FirstName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    LastName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                    BirthDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7)
                                },
                                Genre = new Genre
                                {
                                    Id = reader.GetInt32(8),
                                    Name = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                    Description = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                }
                            });
                        }
                    }
                }
            }

            return books;
        }

        public async Task<List<Book>> GetBooksByGenreAsync(int genreId)
        {
            var books = new List<Book>();

            using (var context = new DatabaseContext(_connectionString))
            {
                var query = @"
                    SELECT b.BookId, b.Title, b.PublicationYear, b.IsAvailable, 
                           a.AuthorId, a.FirstName, a.LastName, a.BirthDate, 
                           g.GenreId, g.Name, g.Description
                    FROM Books b
                    LEFT JOIN Authors a ON b.AuthorId = a.AuthorId
                    LEFT JOIN Genres g ON b.GenreId = g.GenreId
                    WHERE b.GenreId = @GenreId";

                using (var command = new SqlCommand(query, context.Connection))
                {
                    command.Parameters.AddWithValue("@GenreId", genreId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                PublicationYear = reader.GetInt32(2),
                                IsAvailable = reader.GetBoolean(3),
                                Author = new Author
                                {
                                    Id = reader.GetInt32(4),
                                    FirstName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                    LastName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                    BirthDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7)
                                },
                                Genre = new Genre
                                {
                                    Id = reader.GetInt32(8),
                                    Name = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                                    Description = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                                }
                            });
                        }
                    }
                }
            }

            return books;
        }
    }
}