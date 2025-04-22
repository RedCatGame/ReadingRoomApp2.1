using System.Windows.Controls;
using ReadingRoomApp.Presentation.ViewModels.Auth;
using ReadingRoomApp.ViewModels;

namespace ReadingRoomApp.Presentation.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }
    }
}