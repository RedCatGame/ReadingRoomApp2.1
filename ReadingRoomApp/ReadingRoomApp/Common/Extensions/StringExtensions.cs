using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ReadingRoomApp.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string TrimSafe(this string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Используем регулярное выражение для проверки формата email
                // Это простая проверка, в реальном приложении может потребоваться более сложная валидация
                var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static int? ToNullableInt(this string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return null;
        }

        public static DateTime? ToNullableDateTime(this string value)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }
    }
}