using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Common.Constants;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.Events;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public event EventHandler<AuthenticatedUserEventArgs> LoginSuccessful;

        public LoginViewModel(IAuthenticationService authService = null)
        {
            _authService = authService ?? App.AuthenticationService;

            LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
            NavigateToRegisterCommand = new RelayCommand(NavigateToRegister);
        }

        private bool CanLogin(object arg)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync(object obj)
        {
            await ExecuteAsync(async () =>
            {
                // Проверяем входные данные
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пожалуйста, введите имя пользователя и пароль";
                    return;
                }

                // Выполняем аутентификацию
                var user = await _authService.AuthenticateAsync(Username, Password);

                if (user != null)
                {
                    // Публикуем событие входа пользователя
                    App.EventAggregator.Publish(new UserLoggedInEvent(user));

                    // Вызываем обработчик события успешного входа
                    LoginSuccessful?.Invoke(this, new AuthenticatedUserEventArgs(user));

                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = ErrorMessages.InvalidCredentials;
                    App.Logger.LogWarning($"Неудачная попытка входа с именем пользователя: {Username}");
                }
            }, "Ошибка при входе в систему");
        }

        private void NavigateToRegister(object obj)
        {
            // Создаем ViewModel для регистрации (только для читателей, не админов)
            var registerViewModel = ViewModelLocator.Instance.CreateRegisterViewModel(false);
            App.NavigationService.NavigateTo(registerViewModel);
        }
    }

    public class AuthenticatedUserEventArgs : EventArgs
    {
        public User User { get; }

        public AuthenticatedUserEventArgs(User user)
        {
            User = user;
        }
    }
}