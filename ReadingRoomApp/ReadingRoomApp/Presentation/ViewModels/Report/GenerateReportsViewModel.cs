using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.Helpers;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Report
{
    public class GenerateReportsViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;

        private ObservableCollection<Book> _books;
        private ObservableCollection<Reader> _readers;
        private ObservableCollection<Author> _authors;
        private string _selectedReportType;
        private Reader _selectedReader;
        private Author _selectedAuthor;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _reportFormat = "txt";

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

        public string SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                if (SetProperty(ref _selectedReportType, value))
                {
                    OnPropertyChanged(nameof(IsBookListReportSelected));
                    OnPropertyChanged(nameof(IsReaderListReportSelected));
                    OnPropertyChanged(nameof(IsBorrowingReportSelected));
                    OnPropertyChanged(nameof(IsAuthorReportSelected));
                    OnPropertyChanged(nameof(IsGenreStatisticsReportSelected));
                    OnPropertyChanged(nameof(IsActivityReportSelected));
                }
            }
        }

        public Reader SelectedReader
        {
            get => _selectedReader;
            set => SetProperty(ref _selectedReader, value);
        }

        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set => SetProperty(ref _selectedAuthor, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string ReportFormat
        {
            get => _reportFormat;
            set => SetProperty(ref _reportFormat, value);
        }

        public bool IsBookListReportSelected => SelectedReportType == "BookList";
        public bool IsReaderListReportSelected => SelectedReportType == "ReaderList";
        public bool IsBorrowingReportSelected => SelectedReportType == "Borrowing";
        public bool IsAuthorReportSelected => SelectedReportType == "Author";
        public bool IsGenreStatisticsReportSelected => SelectedReportType == "GenreStatistics";
        public bool IsActivityReportSelected => SelectedReportType == "Activity";

        public List<string> ReportTypes { get; } = new List<string>
        {
            "BookList",
            "ReaderList",
            "Borrowing",
            "Author",
            "GenreStatistics",
            "Activity"
        };

        public List<string> ReportFormats { get; } = new List<string>
        {
            "txt",
            "pdf",
            "csv"
        };

        public ICommand GenerateReportCommand { get; }
        public ICommand OpenReportCommand { get; }
        public ICommand BackCommand { get; }

        private string _lastGeneratedReportPath;

        public GenerateReportsViewModel(
            IReportService reportService = null,
            IBookService bookService = null,
            IReaderService readerService = null,
            IAuthorService authorService = null)
        {
            _reportService = reportService ?? App.ReportService;
            _bookService = bookService ?? App.BookService;
            _readerService = readerService ?? App.ReaderService;
            _authorService = authorService ?? App.AuthorService;

            GenerateReportCommand = new AsyncRelayCommand(GenerateReportAsync, CanGenerateReport);
            OpenReportCommand = new RelayCommand(OpenReport, _ => !string.IsNullOrEmpty(_lastGeneratedReportPath));
            BackCommand = new RelayCommand(NavigateBack);

            // Установка значений по умолчанию
            SelectedReportType = "BookList";
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var booksTask = _bookService.GetAllAsync();
                var readersTask = _readerService.GetAllAsync();
                var authorsTask = _authorService.GetAllAsync();

                await Task.WhenAll(booksTask, readersTask, authorsTask);

                Books = new ObservableCollection<Book>(await booksTask);
                Readers = new ObservableCollection<Reader>(await readersTask);
                Authors = new ObservableCollection<Author>(await authorsTask);
            }, "Ошибка при загрузке данных");
        }

        private bool CanGenerateReport(object arg)
        {
            switch (SelectedReportType)
            {
                case "BookList":
                    return Books != null && Books.Any();
                case "ReaderList":
                    return Readers != null && Readers.Any();
                case "Borrowing":
                    return SelectedReader != null;
                case "Author":
                    return SelectedAuthor != null;
                case "GenreStatistics":
                    return true;
                case "Activity":
                    return StartDate <= EndDate;
                default:
                    return false;
            }
        }

        private async Task GenerateReportAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                switch (SelectedReportType)
                {
                    case "BookList":
                        _lastGeneratedReportPath = await _reportService.GenerateBookListReportAsync(Books, ReportFormat);
                        break;
                    case "ReaderList":
                        _lastGeneratedReportPath = await _reportService.GenerateReaderListReportAsync(Readers, ReportFormat);
                        break;
                    case "Borrowing":
                        _lastGeneratedReportPath = await _reportService.GenerateBorrowingReportAsync(SelectedReader.Id, ReportFormat);
                        break;
                    case "Author":
                        _lastGeneratedReportPath = await _reportService.GenerateAuthorReportAsync(SelectedAuthor.Id, ReportFormat);
                        break;
                    case "GenreStatistics":
                        _lastGeneratedReportPath = await _reportService.GenerateGenreStatisticsReportAsync(ReportFormat);
                        break;
                    case "Activity":
                        _lastGeneratedReportPath = await _reportService.GenerateActivityReportAsync(StartDate, EndDate, ReportFormat);
                        break;
                }

                MessageBoxHelper.ShowInformation($"Отчет успешно создан и сохранен в:\n{_lastGeneratedReportPath}", "Отчет готов");
            }, "Ошибка при создании отчета");
        }

        private void OpenReport(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(_lastGeneratedReportPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = _lastGeneratedReportPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при открытии отчета: {ex.Message}");
                MessageBoxHelper.ShowError($"Не удалось открыть отчет: {ex.Message}", "Ошибка");
            }
        }

        private void NavigateBack(object obj)
        {
            App.NavigationService.NavigateBack();
        }
    }
}