using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;
using ReadingRoomApp.Services;

namespace ReadingRoomApp.Presentation.ViewModels.Book
{
    public class AddBookViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;
        private Core.Domain.Entities.Book _newBook;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Genre> _genres;
        private bool _isLoading;

        public Core.Domain.Entities.Book NewBook
        {
            get => _newBook;
            set => SetProperty(ref _newBook, value);
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler BookAdded;
        public event EventHandler CancelRequested;

        public AddBookViewModel(
            IBookService bookService,
            IAuthorService authorService = null,
            IGenreService genreService = null)
        {
            _bookService = bookService ?? App.BookService;
            _authorService = authorService; // TODO: Добавить в App
            _genreService = genreService; // TODO: Добавить в App

            NewBook = new Core.Domain.Entities.Book { IsAvailable = true };
            Authors = new ObservableCollection<Author>();
            Genres = new ObservableCollection<Genre>();

            SaveCommand = new RelayCommand(SaveBook, CanSaveBook);
            CancelCommand = new RelayCommand(Cancel);

            LoadAuthorsAndGenres().ConfigureAwait(false);
        }

        private async Task LoadAuthorsAndGenres()
        {
            IsLoading = true;
            try
            {
                if (_authorService != null)
                {
                    var authors = await _authorService.GetAllAuthorsAsync();
                    Authors.Clear();
                    foreach (var author in authors)
                    {
                        Authors.Add(author);
                    }
                }

                if (_genreService != null)
                {
                    var genres = await _genreService.GetAllGenresAsync();
                    Genres.Clear();
                    foreach (var genre in genres)
                    {
                        Genres.Add(genre);
                    }
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSaveBook(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewBook.Title) &&
                   NewBook.Author != null &&
                   NewBook.Genre != null &&
                   NewBook.PublicationYear > 0;
        }

        private async void SaveBook(object obj)
        {
            await _bookService.AddBookAsync(NewBook);
            BookAdded?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel(object obj)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}