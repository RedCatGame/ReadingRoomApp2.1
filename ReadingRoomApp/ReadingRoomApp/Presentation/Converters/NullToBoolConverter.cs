using System;
using System.Globalization;
using System.Windows.Data;

namespace ReadingRoomApp.Presentation.Converters
{
    /// <summary>
    /// Конвертер для преобразования null в bool. 
    /// Если значение не null, возвращает true, иначе false.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = parameter != null && parameter.ToString().ToLower() == "true";
            bool isNotNull = value != null;

            return invert ? !isNotNull : isNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}