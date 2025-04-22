using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Reader
{
    public class AddReaderViewModel : ViewModelBase
    {
        private readonly IReaderService _readerService;
        private Core.Domain.Entities.Reader _newReader;
        private bool _isLoading;

        public Core.Domain.Entities.Reader NewReader
        {
            get => _newReader;
            set => SetProperty(ref _newReader, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler ReaderAdded;
        public event EventHandler CancelRequested;

        public AddReaderViewModel(IReaderService readerService)
        {
            _readerService = readerService ?? App.ReaderService;
            NewReader = new Core.Domain.Entities.Reader();

            SaveCommand = new RelayCommand(SaveReader, CanSaveReader);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSaveReader(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewReader.FirstName) &&
                   !string.IsNullOrWhiteSpace(NewReader.LastName) &&
                   !string.IsNullOrWhiteSpace(NewReader.Email);
        }

        private async void SaveReader(object obj)
        {
            IsLoading = true;
            try
            {
                await _readerService.AddReaderAsync(NewReader);
                ReaderAdded?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Cancel(object obj)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}