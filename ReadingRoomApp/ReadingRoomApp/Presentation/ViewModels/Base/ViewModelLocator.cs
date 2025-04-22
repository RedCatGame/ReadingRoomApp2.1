using System;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.ViewModels.Auth;
using ReadingRoomApp.Presentation.ViewModels.Book;
using ReadingRoomApp.Presentation.ViewModels.Reader;
using ReadingRoomApp.Presentation.ViewModels.Report;
using ReadingRoomApp.Presentation.ViewModels.Data;

namespace ReadingRoomApp.Presentation.ViewModels.Base
{
    public class ViewModelLocator
    {
        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance => _instance ??= new ViewModelLocator();

        // Book ViewModels
        public BookListViewModel BookListViewModel => new BookListViewModel(App.BookService);

        public BookDetailsViewModel CreateBookDetailsViewModel(Core.Domain.Entities.Book book)
        {
            return new BookDetailsViewModel(book);
        }

        public AddBookViewModel CreateAddBookViewModel()
        {
            return new AddBookViewModel(App.BookService, App.AuthorService, App.GenreService);
        }

        public EditBookViewModel CreateEditBookViewModel(Core.Domain.Entities.Book book)
        {
            return new EditBookViewModel(App.BookService, book, App.AuthorService, App.GenreService);
        }

        // Reader ViewModels
        public ReaderListViewModel ReaderListViewModel => new ReaderListViewModel(App.ReaderService);

        public ReaderDetailsViewModel CreateReaderDetailsViewModel(Core.Domain.Entities.Reader reader)
        {
            return new ReaderDetailsViewModel(reader);
        }

        public AddReaderViewModel CreateAddReaderViewModel()
        {
            return new AddReaderViewModel(App.ReaderService);
        }

        public EditReaderViewModel CreateEditReaderViewModel(Core.Domain.Entities.Reader reader)
        {
            return new EditReaderViewModel(App.ReaderService, reader);
        }

        // Auth ViewModels
        public LoginViewModel LoginViewModel => new LoginViewModel(App.AuthenticationService);

        public RegisterViewModel CreateRegisterViewModel(bool isAdmin)
        {
            return new RegisterViewModel(App.AuthenticationService, App.UserService, isAdmin);
        }

        public GenerateReportsViewModel CreateGenerateReportsViewModel()
        {
            return new GenerateReportsViewModel();
        }

        public DataExchangeViewModel CreateDataExchangeViewModel()
        {
            return new DataExchangeViewModel();
        }

        // Main ViewModel
        public MainWindowViewModel MainWindowViewModel => new MainWindowViewModel(
            App.AuthenticationService,
            App.BookService,
            App.ReaderService
        );
    }
}