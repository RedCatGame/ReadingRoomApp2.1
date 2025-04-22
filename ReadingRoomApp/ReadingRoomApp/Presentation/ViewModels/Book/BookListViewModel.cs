using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;
using ReadingRoomApp.ViewModels;

namespace ReadingRoomApp.Presentation.ViewModels.Book
{
    public class BookListViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private ObservableCollection<Core.Domain.Entities.Book> _books;
        private Core.Domain.Entities.Book _selectedBook;
        private object _currentView;
        private bool _isLoading;

        public ObservableCollection<Core.Domain.Entities.Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public Core.Domain.Entities.Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                SetProperty(ref _currentView, value);
                OnPropertyChanged(nameof(IsListViewVisible));
            }
        }

        public bool IsListViewVisible => CurrentView == null;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand ViewBookDetailsCommand { get; }
        public ICommand RefreshCommand { get; }

        public BookListViewModel(IBookService bookService = null)
        {
            _bookService = bookService ?? App.BookService;
            Books = new ObservableCollection<Core.Domain.Entities.Book>();

            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, CanEditBook);
            DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
            ViewBookDetailsCommand = new RelayCommand(ViewBookDetails, CanViewBookDetails);
            RefreshCommand = new RelayCommand(async _ => await LoadBooksAsync());

            LoadBooksAsync().ConfigureAwait(false);
        }

        private async Task LoadBooksAsync()
        {
            IsLoading = true;
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddBook(object obj)
        {
            var addBookViewModel = new AddBookViewModel(_bookService);
            addBookViewModel.BookAdded += async (s, e) => {
                await LoadBooksAsync();
                CurrentView = null;
            };
            addBookViewModel.CancelRequested += (s, e) => CurrentView = null;
            CurrentView = addBookViewModel;
        }

        private bool CanEditBook(object obj) => SelectedBook != null;

        private void EditBook(object obj)
        {
            if (SelectedBook != null)
            {
                var editBookViewModel = new EditBookViewModel(_bookService, SelectedBook);
                editBookViewModel.BookUpdated += async (s, e) => {
                    await LoadBooksAsync();
                    CurrentView = null;
                };
                editBookViewModel.CancelRequested += (s, e) => CurrentView = null;
                CurrentView = editBookViewModel;
            }
        }

        private bool CanDeleteBook(object obj) => SelectedBook != null;

        private async void DeleteBook(object obj)
        {
            if (SelectedBook != null)
            {
                await _bookService.DeleteBookAsync(SelectedBook.Id);
                Books.Remove(SelectedBook);
            }
        }

        private bool CanViewBookDetails(object obj) => SelectedBook != null;

        private void ViewBookDetails(object obj)
        {
            if (SelectedBook != null)
            {
                var bookDetailsViewModel = new BookDetailsViewModel(SelectedBook);
                bookDetailsViewModel.CloseRequested += (s, e) => CurrentView = null;
                bookDetailsViewModel.EditRequested += (s, book) => EditBook(book);
                CurrentView = bookDetailsViewModel;
            }
        }
    }
}