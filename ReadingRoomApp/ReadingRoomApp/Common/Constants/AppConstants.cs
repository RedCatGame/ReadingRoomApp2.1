using System;
using System.IO;

namespace ReadingRoomApp.Common.Constants
{
    public static class AppConstants
    {
        // Информация о приложении
        public const string AppName = "Reading Room";
        public const string AppVersion = "1.0.0";

        // Пути к директориям
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string DataDirectory = Path.Combine(BaseDirectory, "Data");
        public static readonly string LogsDirectory = Path.Combine(BaseDirectory, "Logs");

        // Пути к файлам
        public const string BOOKS_FILE = "books.json";
        public const string AUTHORS_FILE = "authors.json";
        public const string GENRES_FILE = "genres.json";
        public const string READERS_FILE = "readers.json";
        public const string USERS_FILE = "users.json";

        // Полные пути к файлам
        public static readonly string BOOKS_FILE_PATH = Path.Combine(DataDirectory, BOOKS_FILE);
        public static readonly string AUTHORS_FILE_PATH = Path.Combine(DataDirectory, AUTHORS_FILE);
        public static readonly string GENRES_FILE_PATH = Path.Combine(DataDirectory, GENRES_FILE);
        public static readonly string READERS_FILE_PATH = Path.Combine(DataDirectory, READERS_FILE);
        public static readonly string USERS_FILE_PATH = Path.Combine(DataDirectory, USERS_FILE);
        public static readonly string LOG_FILE_PATH = Path.Combine(LogsDirectory, "app.log");

        // Сообщения об ошибках
        public const string DB_CONNECTION_ERROR = "Ошибка при подключении к базе данных. Будут использованы файловые хранилища.";

        // Настройки интерфейса
        public const int PageSize = 20;

        // Настройки безопасности
        public const int MinPasswordLength = 4;
        public const int MaxUsernameLength = 50;
        public const int MaxEmailLength = 100;
    }
}