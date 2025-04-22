using System;
using System.Globalization;
using System.Windows.Data;

namespace ReadingRoomApp.Presentation.Converters
{
    /// <summary>
    /// Конвертер для преобразования bool в строку доступности.
    /// Если true, возвращает "Доступна", иначе "Не доступна".
    /// </summary>
    public class BoolToAvailabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAvailable)
            {
                return isAvailable ? "Доступна" : "Не доступна";
            }
            return "Не определено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                return strValue == "Доступна";
            }
            return false;
        }
    }
}