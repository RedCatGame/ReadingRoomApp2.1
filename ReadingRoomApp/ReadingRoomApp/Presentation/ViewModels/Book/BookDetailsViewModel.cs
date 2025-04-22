using System;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Book
{
    public class BookDetailsViewModel : ViewModelBase
    {
        private Core.Domain.Entities.Book _book;

        public Core.Domain.Entities.Book Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }

        public event EventHandler CloseRequested;
        public event EventHandler<Core.Domain.Entities.Book> EditRequested;

        public BookDetailsViewModel(Core.Domain.Entities.Book book)
        {
            Book = book ?? throw new ArgumentNullException(nameof(book));

            CloseCommand = new RelayCommand(Close);
            EditCommand = new RelayCommand(Edit);
        }

        private void Close(object obj)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Edit(object obj)
        {
            EditRequested?.Invoke(this, Book);
        }
    }
}