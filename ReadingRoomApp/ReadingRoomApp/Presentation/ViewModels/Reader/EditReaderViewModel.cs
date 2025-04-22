using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Reader
{
    public class EditReaderViewModel : ViewModelBase
    {
        private readonly IReaderService _readerService;
        private Core.Domain.Entities.Reader _readerToEdit;
        private bool _isLoading;

        public Core.Domain.Entities.Reader ReaderToEdit
        {
            get => _readerToEdit;
            set => SetProperty(ref _readerToEdit, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler ReaderUpdated;
        public event EventHandler CancelRequested;

        public EditReaderViewModel(IReaderService readerService, Core.Domain.Entities.Reader readerToEdit)
        {
            _readerService = readerService ?? App.ReaderService;
            ReaderToEdit = readerToEdit ?? throw new ArgumentNullException(nameof(readerToEdit));

            SaveCommand = new RelayCommand(SaveReader, CanSaveReader);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSaveReader(object obj)
        {
            return !string.IsNullOrWhiteSpace(ReaderToEdit.FirstName) &&
                   !string.IsNullOrWhiteSpace(ReaderToEdit.LastName) &&
                   !string.IsNullOrWhiteSpace(ReaderToEdit.Email);
        }

        private async void SaveReader(object obj)
        {
            IsLoading = true;
            try
            {
                await _readerService.UpdateReaderAsync(ReaderToEdit);
                ReaderUpdated?.Invoke(this, EventArgs.Empty);
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