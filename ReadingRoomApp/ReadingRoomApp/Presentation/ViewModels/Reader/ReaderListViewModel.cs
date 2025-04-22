using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;
using ReadingRoomApp.ViewModels;

namespace ReadingRoomApp.Presentation.ViewModels.Reader
{
    public class ReaderListViewModel : ViewModelBase
    {
        private readonly IReaderService _readerService;
        private ObservableCollection<Core.Domain.Entities.Reader> _readers;
        private Core.Domain.Entities.Reader _selectedReader;
        private object _currentView;
        private bool _isLoading;

        public ObservableCollection<Core.Domain.Entities.Reader> Readers
        {
            get => _readers;
            set => SetProperty(ref _readers, value);
        }

        public Core.Domain.Entities.Reader SelectedReader
        {
            get => _selectedReader;
            set => SetProperty(ref _selectedReader, value);
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                SetProperty(ref _currentView, value);
                OnPropertyChanged(nameof(IsListViewVisible));
            }
        }

        public bool IsListViewVisible => CurrentView == null;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand AddReaderCommand { get; }
        public ICommand EditReaderCommand { get; }
        public ICommand DeleteReaderCommand { get; }
        public ICommand ViewReaderDetailsCommand { get; }
        public ICommand RefreshCommand { get; }

        public ReaderListViewModel(IReaderService readerService = null)
        {
            _readerService = readerService ?? App.ReaderService;
            Readers = new ObservableCollection<Core.Domain.Entities.Reader>();

            AddReaderCommand = new RelayCommand(AddReader);
            EditReaderCommand = new RelayCommand(EditReader, CanEditReader);
            DeleteReaderCommand = new RelayCommand(DeleteReader, CanDeleteReader);
            ViewReaderDetailsCommand = new RelayCommand(ViewReaderDetails, CanViewReaderDetails);
            RefreshCommand = new RelayCommand(async _ => await LoadReadersAsync());

            LoadReadersAsync().ConfigureAwait(false);
        }

        private async Task LoadReadersAsync()
        {
            IsLoading = true;
            try
            {
                var readers = await _readerService.GetAllReadersAsync();
                Readers.Clear();
                foreach (var reader in readers)
                {
                    Readers.Add(reader);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddReader(object obj)
        {
            var addReaderViewModel = new AddReaderViewModel(_readerService);
            addReaderViewModel.ReaderAdded += async (s, e) => {
                await LoadReadersAsync();
                CurrentView = null;
            };
            addReaderViewModel.CancelRequested += (s, e) => CurrentView = null;
            CurrentView = addReaderViewModel;
        }

        private bool CanEditReader(object obj) => SelectedReader != null;

        private void EditReader(object obj)
        {
            if (SelectedReader != null)
            {
                var editReaderViewModel = new EditReaderViewModel(_readerService, SelectedReader);
                editReaderViewModel.ReaderUpdated += async (s, e) => {
                    await LoadReadersAsync();
                    CurrentView = null;
                };
                editReaderViewModel.CancelRequested += (s, e) => CurrentView = null;
                CurrentView = editReaderViewModel;
            }
        }

        private bool CanDeleteReader(object obj) => SelectedReader != null;

        private async void DeleteReader(object obj)
        {
            if (SelectedReader != null)
            {
                await _readerService.DeleteReaderAsync(SelectedReader.Id);
                Readers.Remove(SelectedReader);
            }
        }

        private bool CanViewReaderDetails(object obj) => SelectedReader != null;

        private void ViewReaderDetails(object obj)
        {
            if (SelectedReader != null)
            {
                var readerDetailsViewModel = new ReaderDetailsViewModel(SelectedReader);
                readerDetailsViewModel.CloseRequested += (s, e) => CurrentView = null;
                readerDetailsViewModel.EditRequested += (s, reader) => EditReader(reader);
                CurrentView = readerDetailsViewModel;
            }
        }
    }
}