using System;
using System.Windows;

namespace ReadingRoomApp.Presentation.Helpers
{
    public static class MessageBoxHelper
    {
        public static void ShowError(string message, string title = "Ошибка")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public static void ShowWarning(string message, string title = "Предупреждение")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public static void ShowInformation(string message, string title = "Информация")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public static bool ShowQuestion(string message, string title = "Вопрос")
        {
            bool result = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            });
            return result;
        }
    }
}