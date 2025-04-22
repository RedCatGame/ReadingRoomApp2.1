using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReadingRoomApp.Presentation.Converters
{
    /// <summary>
    /// Конвертер для преобразования bool в Visibility.
    /// Если значение true, возвращает Visible, иначе Collapsed.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            if (value is bool v)
            {
                boolValue = v;
            }
            else if (value is string v1)
            {
                boolValue = !string.IsNullOrEmpty(v1);
            }
            else if (value != null)
            {
                boolValue = true;
            }

            bool invert = parameter != null && parameter.ToString().ToLower() == "true";

            if (invert)
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool invert = parameter != null && parameter.ToString().ToLower() == "true";
                bool result = visibility == Visibility.Visible;

                if (invert)
                {
                    result = !result;
                }

                return result;
            }

            return false;
        }
    }
}