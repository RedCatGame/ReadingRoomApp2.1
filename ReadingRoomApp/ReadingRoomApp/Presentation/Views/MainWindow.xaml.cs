using System.Windows;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.Views
{
    /// <summary>
    /// Основное окно приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Записываем в лог информацию о запуске главного окна
            App.Logger.LogInfo("Запуск главного окна приложения");

            // Используем ViewModelLocator для получения ViewModel
            DataContext = ViewModelLocator.Instance.MainWindowViewModel;

            // Подписываемся на событие закрытия окна
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            App.Logger.LogInfo("Завершение работы приложения");
        }
    }
}