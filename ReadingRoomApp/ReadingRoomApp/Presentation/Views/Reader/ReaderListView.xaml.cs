using System.Windows.Controls;
using System.Windows.Input;

namespace ReadingRoomApp.Presentation.Views.Reader
{
    public partial class ReaderListView : UserControl
    {
        public ReaderListView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ViewModels.Reader.ReaderListViewModel viewModel && viewModel.SelectedReader != null)
            {
                viewModel.ViewReaderDetailsCommand.Execute(null);
            }
        }
    }
}