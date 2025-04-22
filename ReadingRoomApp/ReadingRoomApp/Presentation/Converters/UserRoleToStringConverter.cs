using System;
using System.Globalization;
using System.Windows.Data;
using ReadingRoomApp.Core.Domain.Enums;

namespace ReadingRoomApp.Presentation.Converters
{
    /// <summary>
    /// Конвертер для преобразования UserRole в строковое представление.
    /// </summary>
    public class UserRoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
            {
                switch (role)
                {
                    case UserRole.Reader:
                        return "Читатель";
                    case UserRole.Author:
                        return "Писатель";
                    case UserRole.Admin:
                        return "Администратор";
                    default:
                        return "Неизвестно";
                }
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string roleStr)
            {
                switch (roleStr)
                {
                    case "Читатель":
                        return UserRole.Reader;
                    case "Писатель":
                        return UserRole.Author;
                    case "Администратор":
                        return UserRole.Admin;
                    default:
                        return UserRole.Reader;
                }
            }
            return UserRole.Reader;
        }
    }
}