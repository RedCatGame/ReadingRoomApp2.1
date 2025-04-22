using System.Windows.Controls;
using ReadingRoomApp.Presentation.ViewModels.Auth;
using ReadingRoomApp.ViewModels;

namespace ReadingRoomApp.Presentation.Views.Auth
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }
    }
}