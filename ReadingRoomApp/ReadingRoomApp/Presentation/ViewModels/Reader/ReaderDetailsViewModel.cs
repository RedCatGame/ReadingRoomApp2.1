using System;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Reader
{
    public class ReaderDetailsViewModel : ViewModelBase
    {
        private Core.Domain.Entities.Reader _reader;

        public Core.Domain.Entities.Reader Reader
        {
            get => _reader;
            set => SetProperty(ref _reader, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }

        public event EventHandler CloseRequested;
        public event EventHandler<Core.Domain.Entities.Reader> EditRequested;

        public ReaderDetailsViewModel(Core.Domain.Entities.Reader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));

            CloseCommand = new RelayCommand(Close);
            EditCommand = new RelayCommand(Edit);
        }

        private void Close(object obj)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Edit(object obj)
        {
            EditRequested?.Invoke(this, Reader);
        }
    }
}