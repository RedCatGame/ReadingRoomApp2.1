using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Presentation.Validation
{
    public class BookValidator : IValidator<Book>
    {
        public ValidationResult Validate(Book book)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                result.AddError(nameof(book.Title), "Название книги обязательно для заполнения");
            }
            else if (book.Title.Length > 255)
            {
                result.AddError(nameof(book.Title), "Название книги не должно превышать 255 символов");
            }

            if (book.Author == null)
            {
                result.AddError(nameof(book.Author), "Необходимо выбрать автора");
            }

            if (book.Genre == null)
            {
                result.AddError(nameof(book.Genre), "Необходимо выбрать жанр");
            }

            if (book.PublicationYear <= 0)
            {
                result.AddError(nameof(book.PublicationYear), "Год издания должен быть положительным числом");
            }
            else if (book.PublicationYear < 1000 || book.PublicationYear > 2100)
            {
                result.AddError(nameof(book.PublicationYear), "Год издания должен быть в диапазоне от 1000 до 2100");
            }

            return result;
        }
    }
}