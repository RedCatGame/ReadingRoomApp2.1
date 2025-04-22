using System;
using System.Threading.Tasks;
using ReadingRoomApp.Core.Interfaces.Repositories;

namespace ReadingRoomApp.Infrastructure.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly DatabaseContext _context;

        public IBookRepository Books { get; }
        public IAuthorRepository Authors { get; }
        public IGenreRepository Genres { get; }
        public IReaderRepository Readers { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(string connectionString,
                         IBookRepository bookRepository,
                         IAuthorRepository authorRepository,
                         IGenreRepository genreRepository,
                         IReaderRepository readerRepository,
                         IUserRepository userRepository)
        {
            _context = new DatabaseContext(connectionString);

            Books = bookRepository;
            Authors = authorRepository;
            Genres = genreRepository;
            Readers = readerRepository;
            Users = userRepository;
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}