using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Domain.Enums;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.Events;
using ReadingRoomApp.Presentation.ViewModels.Auth;
using ReadingRoomApp.Presentation.ViewModels.Base;
using ReadingRoomApp.Presentation.ViewModels.Book;
using ReadingRoomApp.Presentation.ViewModels.Reader;

namespace ReadingRoomApp.Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;

        private object _currentView;
        private User _currentUser;
        private string _statusMessage;
        private string _loadingText = "Загрузка...";

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                if (SetProperty(ref _currentUser, value))
                {
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(IsAdmin));
                    OnPropertyChanged(nameof(IsAuthor));
                    OnPropertyChanged(nameof(IsReader));
                }
            }
        }

        public bool IsLoggedIn => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
        public bool IsAuthor => CurrentUser?.Role == UserRole.Author;
        public bool IsReader => CurrentUser?.Role == UserRole.Reader;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string LoadingText
        {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        // Команды навигации
        public ICommand NavigateToBookListCommand { get; }
        public ICommand NavigateToAddBookCommand { get; }
        public ICommand NavigateToMyBooksCommand { get; }
        public ICommand NavigateToReaderListCommand { get; }
        public ICommand NavigateToAddReaderCommand { get; }
        public ICommand NavigateToAuthorsCommand { get; }
        public ICommand NavigateToGenresCommand { get; }
        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToMyLibraryCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand NavigateToReportsCommand { get; }
        public ICommand NavigateToDataExchangeCommand { get; }

        public MainWindowViewModel(
            IAuthenticationService authService = null,
            IBookService bookService = null,
            IReaderService readerService = null)
        {
            _authService = authService ?? App.AuthenticationService;
            _bookService = bookService ?? App.BookService;
            _readerService = readerService ?? App.ReaderService;

            // Команды навигации
            NavigateToBookListCommand = new RelayCommand(NavigateToBookList);
            NavigateToAddBookCommand = new RelayCommand(NavigateToAddBook, _ => IsLoggedIn && (IsAdmin || IsAuthor));
            NavigateToMyBooksCommand = new RelayCommand(NavigateToMyBooks, _ => IsLoggedIn && IsAuthor);
            NavigateToReaderListCommand = new RelayCommand(NavigateToReaderList, _ => IsLoggedIn && IsAdmin);
            NavigateToAddReaderCommand = new RelayCommand(NavigateToAddReader, _ => IsLoggedIn && IsAdmin);
            NavigateToAuthorsCommand = new RelayCommand(NavigateToAuthors, _ => IsLoggedIn && IsAdmin);
            NavigateToGenresCommand = new RelayCommand(NavigateToGenres, _ => IsLoggedIn && IsAdmin);
            NavigateToUsersCommand = new RelayCommand(NavigateToUsers, _ => IsLoggedIn && IsAdmin);
            NavigateToProfileCommand = new RelayCommand(NavigateToProfile, _ => IsLoggedIn);
            NavigateToMyLibraryCommand = new RelayCommand(NavigateToMyLibrary, _ => IsLoggedIn && IsReader);
            LogoutCommand = new RelayCommand(Logout, _ => IsLoggedIn);
            NavigateToReportsCommand = new RelayCommand(NavigateToReports, _ => IsLoggedIn && IsAdmin);
            NavigateToDataExchangeCommand = new RelayCommand(NavigateToDataExchange, _ => IsLoggedIn && IsAdmin);


            // Подписываемся на события
            App.EventAggregator.Subscribe<UserLoggedInEvent>(OnUserLoggedIn);
            App.EventAggregator.Subscribe<UserLoggedOutEvent>(OnUserLoggedOut);
            App.EventAggregator.Subscribe<BookCreatedEvent>(OnBookCreated);
            App.EventAggregator.Subscribe<BookUpdatedEvent>(OnBookUpdated);
            App.EventAggregator.Subscribe<BookDeletedEvent>(OnBookDeleted);
            App.EventAggregator.Subscribe<ReaderCreatedEvent>(OnReaderCreated);
            App.EventAggregator.Subscribe<ReaderUpdatedEvent>(OnReaderUpdated);
            App.EventAggregator.Subscribe<ReaderDeletedEvent>(OnReaderDeleted);

            // Показываем экран логина при запуске
            ShowLoginScreen();
        }

        #region Event Handlers

        private void OnUserLoggedIn(UserLoggedInEvent evt)
        {
            CurrentUser = evt.User;
            App.Logger.LogInfo($"Пользователь {evt.User.Username} вошел в систему");
            StatusMessage = $"Добро пожаловать, {evt.User.Username}!";
            NavigateToBookList(null);
        }

        private void OnUserLoggedOut(UserLoggedOutEvent evt)
        {
            App.Logger.LogInfo("Пользователь вышел из системы");
            StatusMessage = "Сеанс завершен";
        }

        private void OnBookCreated(BookCreatedEvent evt)
        {
            App.Logger.LogInfo($"Создана новая книга: {evt.Book.Title}");
            StatusMessage = $"Книга '{evt.Book.Title}' успешно добавлена";
        }

        private void OnBookUpdated(BookUpdatedEvent evt)
        {
            App.Logger.LogInfo($"Обновлена книга: {evt.Book.Title}");
            StatusMessage = $"Книга '{evt.Book.Title}' успешно обновлена";
        }

        private void OnBookDeleted(BookDeletedEvent evt)
        {
            App.Logger.LogInfo($"Удалена книга с ID: {evt.BookId}");
            StatusMessage = "Книга успешно удалена";
        }

        private void OnReaderCreated(ReaderCreatedEvent evt)
        {
            App.Logger.LogInfo($"Создан новый читатель: {evt.Reader.FullName}");
            StatusMessage = $"Читатель '{evt.Reader.FullName}' успешно добавлен";
        }

        private void OnReaderUpdated(ReaderUpdatedEvent evt)
        {
            App.Logger.LogInfo($"Обновлен читатель: {evt.Reader.FullName}");
            StatusMessage = $"Читатель '{evt.Reader.FullName}' успешно обновлен";
        }

        private void OnReaderDeleted(ReaderDeletedEvent evt)
        {
            App.Logger.LogInfo($"Удален читатель с ID: {evt.ReaderId}");
            StatusMessage = "Читатель успешно удален";
        }

        #endregion

        #region Navigation Methods

        private void ShowLoginScreen()
        {
            var loginViewModel = ViewModelLocator.Instance.LoginViewModel;
            loginViewModel.LoginSuccessful += (sender, args) => CurrentUser = args.User;
            CurrentView = loginViewModel;
        }

        private void NavigateToBookList(object obj)
        {
            CurrentView = ViewModelLocator.Instance.BookListViewModel;
            StatusMessage = "Список книг";
        }

        private void NavigateToAddBook(object obj)
        {
            CurrentView = ViewModelLocator.Instance.CreateAddBookViewModel();
            StatusMessage = "Добавление новой книги";
        }

        private void NavigateToMyBooks(object obj)
        {
            // Здесь можно добавить фильтр только для книг данного автора
            CurrentView = ViewModelLocator.Instance.BookListViewModel;
            StatusMessage = "Мои книги";
        }

        private void NavigateToReaderList(object obj)
        {
            CurrentView = ViewModelLocator.Instance.ReaderListViewModel;
            StatusMessage = "Список читателей";
        }

        private void NavigateToAddReader(object obj)
        {
            CurrentView = ViewModelLocator.Instance.CreateAddReaderViewModel();
            StatusMessage = "Добавление нового читателя";
        }

        private void NavigateToAuthors(object obj)
        {
            // В будущих реализациях
            StatusMessage = "Управление авторами";
        }

        private void NavigateToGenres(object obj)
        {
            // В будущих реализациях
            StatusMessage = "Управление жанрами";
        }

        private void NavigateToUsers(object obj)
        {
            // В будущих реализациях
            StatusMessage = "Управление пользователями";
        }

        private void NavigateToProfile(object obj)
        {
            // В будущих реализациях
            StatusMessage = "Мой профиль";
        }

        private void NavigateToMyLibrary(object obj)
        {
            // В будущих реализациях
            StatusMessage = "Моя библиотека";
        }
        private void NavigateToReports(object obj)
        {
            CurrentView = ViewModelLocator.Instance.CreateGenerateReportsViewModel();
            StatusMessage = "Генерация отчетов";
        }

        private void NavigateToDataExchange(object obj)
        {
            CurrentView = ViewModelLocator.Instance.CreateDataExchangeViewModel();
            StatusMessage = "Импорт/Экспорт данных";
        }

        private void Logout(object obj)
        {
            App.EventAggregator.Publish(new UserLoggedOutEvent());
            CurrentUser = null;
            ShowLoginScreen();
        }

        #endregion
    }
}