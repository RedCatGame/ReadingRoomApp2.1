using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.Helpers;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Data
{
    public class DataExchangeViewModel : ViewModelBase
    {
        private readonly IDataExchangeService _dataExchangeService;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;
        private readonly IGenreService _genreService;

        private ObservableCollection<Book> _books;
        private ObservableCollection<Reader> _readers;
        private ObservableCollection<Author> _authors;
        private ObservableCollection<Genre> _genres;

        private string _selectedDataType;
        private string _selectedExportFormat = "json";
        private string _lastExportPath;
        private string _lastImportPath;
        private string _selectedAction = "export"; // export, import, backup, restore

        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ObservableCollection<Reader> Readers
        {
            get => _readers;
            set => SetProperty(ref _readers, value);
        }

        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        public string SelectedDataType
        {
            get => _selectedDataType;
            set => SetProperty(ref _selectedDataType, value);
        }

        public string SelectedExportFormat
        {
            get => _selectedExportFormat;
            set => SetProperty(ref _selectedExportFormat, value);
        }

        public string SelectedAction
        {
            get => _selectedAction;
            set
            {
                if (SetProperty(ref _selectedAction, value))
                {
                    OnPropertyChanged(nameof(IsExportSelected));
                    OnPropertyChanged(nameof(IsImportSelected));
                    OnPropertyChanged(nameof(IsBackupSelected));
                    OnPropertyChanged(nameof(IsRestoreSelected));
                }
            }
        }

        public bool IsExportSelected => SelectedAction == "export";
        public bool IsImportSelected => SelectedAction == "import";
        public bool IsBackupSelected => SelectedAction == "backup";
        public bool IsRestoreSelected => SelectedAction == "restore";

        public List<string> DataTypes { get; } = new List<string>
        {
            "Books",
            "Readers",
            "Authors",
            "Genres"
        };

        public List<string> ExportFormats { get; } = new List<string>
        {
            "json",
            "csv"
        };

        public List<string> Actions { get; } = new List<string>
        {
            "export",
            "import",
            "backup",
            "restore"
        };

        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand BackupCommand { get; }
        public ICommand RestoreCommand { get; }
        public ICommand BrowseImportFileCommand { get; }
        public ICommand BrowseBackupFolderCommand { get; }
        public ICommand OpenExportFileCommand { get; }
        public ICommand BackCommand { get; }

        public DataExchangeViewModel(
            IDataExchangeService dataExchangeService = null,
            IBookService bookService = null,
            IReaderService readerService = null,
            IAuthorService authorService = null,
            IGenreService genreService = null)
        {
            _dataExchangeService = dataExchangeService ?? App.DataExchangeService;
            _bookService = bookService ?? App.BookService;
            _readerService = readerService ?? App.ReaderService;
            _authorService = authorService ?? App.AuthorService;
            _genreService = genreService ?? App.GenreService;

            ExportCommand = new AsyncRelayCommand(ExportDataAsync);
            ImportCommand = new AsyncRelayCommand(ImportDataAsync);
            BackupCommand = new AsyncRelayCommand(BackupDataAsync);
            RestoreCommand = new AsyncRelayCommand(RestoreDataAsync);
            BrowseImportFileCommand = new AsyncRelayCommand(BrowseImportFileAsync);
            BrowseBackupFolderCommand = new AsyncRelayCommand(BrowseBackupFolderAsync);
            OpenExportFileCommand = new RelayCommand(OpenExportFile, _ => !string.IsNullOrEmpty(_lastExportPath));
            BackCommand = new RelayCommand(NavigateBack);

            // Установка значений по умолчанию
            SelectedDataType = "Books";
            SelectedExportFormat = "json";
            SelectedAction = "export";

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var booksTask = _bookService.GetAllAsync();
                var readersTask = _readerService.GetAllAsync();
                var authorsTask = _authorService.GetAllAsync();
                var genresTask = _genreService.GetAllAsync();

                await Task.WhenAll(booksTask, readersTask, authorsTask, genresTask);

                Books = new ObservableCollection<Book>(await booksTask);
                Readers = new ObservableCollection<Reader>(await readersTask);
                Authors = new ObservableCollection<Author>(await authorsTask);
                Genres = new ObservableCollection<Genre>(await genresTask);
            }, "Ошибка при загрузке данных");
        }

        private async Task ExportDataAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                switch (SelectedDataType)
                {
                    case "Books":
                        _lastExportPath = await _dataExchangeService.ExportBooksAsync(Books, SelectedExportFormat);
                        break;
                    case "Readers":
                        _lastExportPath = await _dataExchangeService.ExportReadersAsync(Readers, SelectedExportFormat);
                        break;
                    case "Authors":
                        _lastExportPath = await _dataExchangeService.ExportAuthorsAsync(Authors, SelectedExportFormat);
                        break;
                    case "Genres":
                        _lastExportPath = await _dataExchangeService.ExportGenresAsync(Genres, SelectedExportFormat);
                        break;
                }

                MessageBoxHelper.ShowInformation($"Данные успешно экспортированы в файл:\n{_lastExportPath}", "Экспорт завершен");
            }, "Ошибка при экспорте данных");
        }

        private async Task ImportDataAsync(object obj)
        {
            if (string.IsNullOrEmpty(_lastImportPath))
            {
                await BrowseImportFileAsync(null);
                if (string.IsNullOrEmpty(_lastImportPath))
                {
                    return;
                }
            }

            await ExecuteAsync(async () =>
            {
                switch (SelectedDataType)
                {
                    case "Books":
                        var importedBooks = await _dataExchangeService.ImportBooksAsync(_lastImportPath);
                        Books = new ObservableCollection<Book>(await _bookService.GetAllAsync());
                        MessageBoxHelper.ShowInformation($"Импортировано {importedBooks.Count()} книг", "Импорт завершен");
                        break;
                    case "Readers":
                        var importedReaders = await _dataExchangeService.ImportReadersAsync(_lastImportPath);
                        Readers = new ObservableCollection<Reader>(await _readerService.GetAllAsync());
                        MessageBoxHelper.ShowInformation($"Импортировано {importedReaders.Count()} читателей", "Импорт завершен");
                        break;
                    case "Authors":
                        var importedAuthors = await _dataExchangeService.ImportAuthorsAsync(_lastImportPath);
                        Authors = new ObservableCollection<Author>(await _authorService.GetAllAsync());
                        MessageBoxHelper.ShowInformation($"Импортировано {importedAuthors.Count()} авторов", "Импорт завершен");
                        break;
                    case "Genres":
                        var importedGenres = await _dataExchangeService.ImportGenresAsync(_lastImportPath);
                        Genres = new ObservableCollection<Genre>(await _genreService.GetAllAsync());
                        MessageBoxHelper.ShowInformation($"Импортировано {importedGenres.Count()} жанров", "Импорт завершен");
                        break;
                }
            }, "Ошибка при импорте данных");
        }

        private async Task BackupDataAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                await _dataExchangeService.BackupDatabaseAsync();
                MessageBoxHelper.ShowInformation("Резервная копия базы данных успешно создана", "Резервное копирование");
            }, "Ошибка при создании резервной копии");
        }

        private async Task RestoreDataAsync(object obj)
        {
            string backupPath = null;

            await ExecuteAsync(async () =>
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Выберите директорию с резервной копией"
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    backupPath = dialog.SelectedPath;
                }
            }, "Ошибка при выборе директории");

            if (string.IsNullOrEmpty(backupPath))
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                if (MessageBoxHelper.ShowQuestion("Вы уверены, что хотите восстановить данные из резервной копии? Существующие данные будут перезаписаны.", "Восстановление данных"))
                {
                    await _dataExchangeService.RestoreDatabaseAsync(backupPath);

                    // Обновляем коллекции данных
                    Books = new ObservableCollection<Book>(await _bookService.GetAllAsync());
                    Readers = new ObservableCollection<Reader>(await _readerService.GetAllAsync());
                    Authors = new ObservableCollection<Author>(await _authorService.GetAllAsync());
                    Genres = new ObservableCollection<Genre>(await _genreService.GetAllAsync());

                    MessageBoxHelper.ShowInformation("Данные успешно восстановлены из резервной копии", "Восстановление данных");
                }
            }, "Ошибка при восстановлении данных");
        }

        private async Task BrowseImportFileAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                string fileFilter = SelectedDataType switch
                {
                    "Books" => "Файлы данных книг|*.json;*.csv|JSON файлы|*.json|CSV файлы|*.csv|Все файлы|*.*",
                    "Readers" => "Файлы данных читателей|*.json;*.csv|JSON файлы|*.json|CSV файлы|*.csv|Все файлы|*.*",
                    "Authors" => "Файлы данных авторов|*.json;*.csv|JSON файлы|*.json|CSV файлы|*.csv|Все файлы|*.*",
                    "Genres" => "Файлы данных жанров|*.json;*.csv|JSON файлы|*.json|CSV файлы|*.csv|Все файлы|*.*",
                    _ => "Все файлы|*.*"
                };

                _lastImportPath = await App.FileService.SelectFileAsync($"Выберите файл для импорта {SelectedDataType}", fileFilter);
            }, "Ошибка при выборе файла");
        }

        private async Task BrowseBackupFolderAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Выберите директорию для сохранения резервной копии"
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await _dataExchangeService.BackupDatabaseAsync(dialog.SelectedPath);
                    MessageBoxHelper.ShowInformation($"Резервная копия сохранена в:\n{dialog.SelectedPath}", "Резервное копирование");
                }
            }, "Ошибка при выборе директории");
        }

        private void OpenExportFile(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(_lastExportPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _lastExportPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при открытии файла: {ex.Message}");
                MessageBoxHelper.ShowError($"Не удалось открыть файл: {ex.Message}", "Ошибка");
            }
        }

        private void NavigateBack(object obj)
        {
            App.NavigationService.NavigateBack();
        }
    }
}