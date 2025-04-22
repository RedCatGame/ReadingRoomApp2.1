using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Presentation.Events
{
    public class UserLoggedInEvent
    {
        public User User { get; }

        public UserLoggedInEvent(User user)
        {
            User = user;
        }
    }

    public class UserLoggedOutEvent
    {
    }

    public class BookCreatedEvent
    {
        public Book Book { get; }

        public BookCreatedEvent(Book book)
        {
            Book = book;
        }
    }

    public class BookUpdatedEvent
    {
        public Book Book { get; }

        public BookUpdatedEvent(Book book)
        {
            Book = book;
        }
    }

    public class BookDeletedEvent
    {
        public int BookId { get; }

        public BookDeletedEvent(int bookId)
        {
            BookId = bookId;
        }
    }

    public class ReaderCreatedEvent
    {
        public Reader Reader { get; }

        public ReaderCreatedEvent(Reader reader)
        {
            Reader = reader;
        }
    }

    public class ReaderUpdatedEvent
    {
        public Reader Reader { get; }

        public ReaderUpdatedEvent(Reader reader)
        {
            Reader = reader;
        }
    }

    public class ReaderDeletedEvent
    {
        public int ReaderId { get; }

        public ReaderDeletedEvent(int readerId)
        {
            ReaderId = readerId;
        }
    }
}