using System.Windows.Controls;
using System.Windows.Input;

namespace ReadingRoomApp.Presentation.Views.Book
{
    public partial class BookListView : UserControl
    {
        public BookListView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ViewModels.Book.BookListViewModel viewModel && viewModel.SelectedBook != null)
            {
                viewModel.ViewBookDetailsCommand.Execute(null);
            }
        }
    }
}