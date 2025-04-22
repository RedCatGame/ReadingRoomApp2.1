using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ReadingRoomApp.Common.Constants;
using ReadingRoomApp.Common.Logging;
using ReadingRoomApp.Common.Services;
using ReadingRoomApp.Core.Interfaces.Repositories;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Core.Services;
using ReadingRoomApp.Infrastructure.Data.Repositories;
using ReadingRoomApp.Infrastructure.Services;
using ReadingRoomApp.Presentation.Events;
using ReadingRoomApp.Presentation.Helpers;
using ReadingRoomApp.Presentation.Services;

namespace ReadingRoomApp
{
    public partial class App : Application
    {
        // Строка подключения к базе данных
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ReadingRoomDB"]?.ConnectionString;

        // Репозитории
        public static IBookRepository BookRepository { get; private set; }
        public static IAuthorRepository AuthorRepository { get; private set; }
        public static IGenreRepository GenreRepository { get; private set; }
        public static IReaderRepository ReaderRepository { get; private set; }
        public static IUserRepository UserRepository { get; private set; }

        // Сервисы приложения
        public static IAuthenticationService AuthenticationService { get; private set; }
        public static IBookService BookService { get; private set; }
        public static IReaderService ReaderService { get; private set; }
        public static IUserService UserService { get; private set; }
        public static IAuthorService AuthorService { get; private set; }
        public static IGenreService GenreService { get; private set; }

        // Общие сервисы
        public static ILogger Logger { get; private set; }
        public static INavigationService NavigationService { get; private set; }
        public static IEventAggregator EventAggregator { get; private set; }
        public static IResourceService ResourceService { get; private set; }
        public static IDateTimeProvider DateTimeProvider { get; private set; }
        public static IFileService FileService { get; private set; }
        public static IReportService ReportService { get; private set; }

        public static IDataExchangeService DataExchangeService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Регистрация обработчика необработанных исключений
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);

            // Инициализация логгера
            Logger = new FileLogger(AppConstants.LOG_FILE_PATH);
            Logger.LogInfo($"Приложение {AppConstants.AppName} v{AppConstants.AppVersion} запущено");

            // Инициализация общих сервисов
            NavigationService = new NavigationService();
            EventAggregator = new EventAggregator();
            ResourceService = new ResourceService();
            DateTimeProvider = new DateTimeProvider();
            FileService = new FileService();

            // Создаем директории для данных и логов, если их нет
            EnsureDirectoriesExist();

            // Инициализация репозиториев и сервисов
            InitializeServices();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogCritical($"Необработанное исключение: {e.Exception.Message}");
            Logger.LogCritical($"StackTrace: {e.Exception.StackTrace}");

            MessageBoxHelper.ShowError($"Произошла непредвиденная ошибка: {e.Exception.Message}\n\nПриложение будет закрыто.", "Критическая ошибка");

            e.Handled = true;
            Environment.Exit(1);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.LogCritical($"Необработанное исключение в Task: {e.Exception.Message}");
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Logger.LogCritical($"Необработанное исключение в домене: {ex.Message}");
            }
        }

        private void EnsureDirectoriesExist()
        {
            // Создаем директорию для данных
            if (!Directory.Exists(AppConstants.DataDirectory))
            {
                Directory.CreateDirectory(AppConstants.DataDirectory);
                Logger.LogInfo($"Создана директория для данных: {AppConstants.DataDirectory}");
            }

            // Создаем директорию для логов
            if (!Directory.Exists(AppConstants.LogsDirectory))
            {
                Directory.CreateDirectory(AppConstants.LogsDirectory);
                Logger.LogInfo($"Создана директория для логов: {AppConstants.LogsDirectory}");
            }

            // Создаем директорию для отчетов
            string reportsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
            if (!Directory.Exists(reportsDirectory))
            {
                Directory.CreateDirectory(reportsDirectory);
                Logger.LogInfo($"Создана директория для отчетов: {reportsDirectory}");
            }
        }

        private void InitializeServices()
        {
            // Если есть строка подключения, используем сервисы для базы данных
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                try
                {
                    InitializeDatabaseServices();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Ошибка при подключении к базе данных: {ex.Message}");
                    MessageBoxHelper.ShowWarning(AppConstants.DB_CONNECTION_ERROR, "Предупреждение");

                    InitializeFileServices();
                }
            }
            else
            {
                // Если строки подключения нет, используем файловые хранилища
                Logger.LogInfo("Строка подключения к БД не найдена. Используются файловые хранилища.");
                InitializeFileServices();
            }
        }

        private void InitializeDatabaseServices()
        {
            try
            {
                Logger.LogInfo("Инициализация сервисов базы данных");

                // Инициализация репозиториев для базы данных
                BookRepository = new BookRepository(ConnectionString);
                AuthorRepository = new AuthorRepository(ConnectionString);
                GenreRepository = new GenreRepository(ConnectionString);
                ReaderRepository = new ReaderRepository(ConnectionString);
                UserRepository = new UserRepository(ConnectionString);

                // Инициализация сервисов
                BookService = new BookService(BookRepository);
                ReaderService = new ReaderService(ReaderRepository, BookRepository);
                UserService = new UserService(UserRepository);
                AuthorService = new AuthorService(AuthorRepository);
                GenreService = new GenreService(GenreRepository);
                AuthenticationService = new AuthenticationService(UserRepository);
                ReportService = new ReportService(BookService, ReaderService, AuthorService, GenreService);
                DataExchangeService = new DataExchangeService(BookService, ReaderService, AuthorService, GenreService);

                Logger.LogInfo("Сервисы базы данных успешно инициализированы");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при инициализации сервисов базы данных: {ex.Message}");
                throw;
            }
        }

        private void InitializeFileServices()
        {
            try
            {
                Logger.LogInfo("Инициализация файловых сервисов");

                // Инициализация репозиториев для файлов
                BookRepository = new FileBookRepository();
                AuthorRepository = new FileAuthorRepository();
                GenreRepository = new FileGenreRepository();
                ReaderRepository = new FileReaderRepository();
                UserRepository = new FileUserRepository();

                // Инициализация сервисов
                BookService = new BookService(BookRepository);
                ReaderService = new ReaderService(ReaderRepository, BookRepository);
                UserService = new UserService(UserRepository);
                AuthorService = new AuthorService(AuthorRepository);
                GenreService = new GenreService(GenreRepository);
                AuthenticationService = new AuthenticationService(UserRepository);
                ReportService = new ReportService(BookService, ReaderService, AuthorService, GenreService);
                DataExchangeService = new DataExchangeService(BookService, ReaderService, AuthorService, GenreService);

                Logger.LogInfo("Файловые сервисы успешно инициализированы");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при инициализации файловых сервисов: {ex.Message}");
                throw;
            }
        }
    }
}