using ReadingRoomApp.Common.Extensions;
using ReadingRoomApp.Core.Domain.Entities;

namespace ReadingRoomApp.Presentation.Validation
{
    public class ReaderValidator : IValidator<Reader>
    {
        public ValidationResult Validate(Reader reader)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(reader.FirstName))
            {
                result.AddError(nameof(reader.FirstName), "Имя обязательно для заполнения");
            }
            else if (reader.FirstName.Length > 100)
            {
                result.AddError(nameof(reader.FirstName), "Имя не должно превышать 100 символов");
            }

            if (string.IsNullOrWhiteSpace(reader.LastName))
            {
                result.AddError(nameof(reader.LastName), "Фамилия обязательна для заполнения");
            }
            else if (reader.LastName.Length > 100)
            {
                result.AddError(nameof(reader.LastName), "Фамилия не должна превышать 100 символов");
            }

            if (string.IsNullOrWhiteSpace(reader.Email))
            {
                result.AddError(nameof(reader.Email), "Email обязателен для заполнения");
            }
            else if (!reader.Email.IsValidEmail())
            {
                result.AddError(nameof(reader.Email), "Некорректный формат email");
            }
            else if (reader.Email.Length > 255)
            {
                result.AddError(nameof(reader.Email), "Email не должен превышать 255 символов");
            }

            if (!string.IsNullOrWhiteSpace(reader.PhoneNumber) && reader.PhoneNumber.Length > 20)
            {
                result.AddError(nameof(reader.PhoneNumber), "Номер телефона не должен превышать 20 символов");
            }

            return result;
        }
    }
}