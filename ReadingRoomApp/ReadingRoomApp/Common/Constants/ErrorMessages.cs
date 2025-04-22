namespace ReadingRoomApp.Common.Constants
{
    public static class ErrorMessages
    {
        // Общие ошибки
        public const string UnexpectedError = "Произошла непредвиденная ошибка";
        public const string ValidationError = "Пожалуйста, исправьте ошибки валидации";
        public const string DataLoadError = "Ошибка при загрузке данных";
        public const string DataSaveError = "Ошибка при сохранении данных";
        public const string DataDeleteError = "Ошибка при удалении данных";

        // Ошибки аутентификации
        public const string LoginFailed = "Ошибка входа в систему. Проверьте имя пользователя и пароль";
        public const string RegistrationFailed = "Ошибка при регистрации";
        public const string UsernameTaken = "Имя пользователя уже занято";
        public const string EmailTaken = "Email уже используется";
        public const string InvalidCredentials = "Неверное имя пользователя или пароль";

        // Ошибки книг
        public const string BookNotFound = "Книга не найдена";
        public const string BookCreateError = "Ошибка при создании книги";
        public const string BookUpdateError = "Ошибка при обновлении книги";
        public const string BookDeleteError = "Ошибка при удалении книги";

        // Ошибки читателей
        public const string ReaderNotFound = "Читатель не найден";
        public const string ReaderCreateError = "Ошибка при создании читателя";
        public const string ReaderUpdateError = "Ошибка при обновлении читателя";
        public const string ReaderDeleteError = "Ошибка при удалении читателя";
        public const string ReaderHasBorrowedBooks = "Нельзя удалить читателя, у которого есть взятые книги";

        // Ошибки заимствования
        public const string BookNotAvailable = "Книга недоступна для выдачи";
        public const string BorrowError = "Ошибка при выдаче книги";
        public const string ReturnError = "Ошибка при возврате книги";
    }
}