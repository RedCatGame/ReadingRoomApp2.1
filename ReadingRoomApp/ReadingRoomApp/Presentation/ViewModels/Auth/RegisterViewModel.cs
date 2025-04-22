using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using ReadingRoomApp.Core.Domain.Entities;
using ReadingRoomApp.Core.Domain.Enums;
using ReadingRoomApp.Core.Interfaces.Services;
using ReadingRoomApp.Presentation.Commands;
using ReadingRoomApp.Presentation.ViewModels.Base;

namespace ReadingRoomApp.Presentation.ViewModels.Auth
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;

        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _email;
        private string _firstName;
        private string _lastName;
        private UserRole _selectedRole = UserRole.Reader;
        private string _errorMessage;
        private List<UserRole> _availableRoles;
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateForm();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidateForm();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ValidateForm();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateForm();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                ValidateForm();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                ValidateForm();
            }
        }

        public UserRole SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }

        public List<UserRole> AvailableRoles
        {
            get => _availableRoles;
            set => SetProperty(ref _availableRoles, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool CanRegister { get; private set; }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public event EventHandler RegistrationSuccessful;
        public event EventHandler CancelRequested;
        public event EventHandler LoginRequested;

        public RegisterViewModel(IAuthenticationService authService, IUserService userService, bool isAdmin = false)
        {
            _authService = authService ?? App.AuthenticationService;
            _userService = userService ?? App.UserService;

            // Определяем доступные роли для выбора
            AvailableRoles = new List<UserRole>();

            // Все пользователи могут зарегистрироваться как читатели или писатели
            AvailableRoles.Add(UserRole.Reader);
            AvailableRoles.Add(UserRole.Author);

            // Только администраторы могут создавать новых администраторов
            if (isAdmin)
            {
                AvailableRoles.Add(UserRole.Admin);
            }

            RegisterCommand = new AsyncRelayCommand(RegisterAsync, _ => CanRegister && !IsLoading);
            CancelCommand = new RelayCommand(_ => CancelRequested?.Invoke(this, EventArgs.Empty));
            NavigateToLoginCommand = new RelayCommand(_ => LoginRequested?.Invoke(this, EventArgs.Empty));
        }

        private async Task RegisterAsync(object obj)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Проверка уникальности имени пользователя
                if (!await _userService.IsUsernameUniqueAsync(Username))
                {
                    ErrorMessage = "Имя пользователя уже занято";
                    return;
                }

                // Проверка уникальности email
                if (!await _userService.IsEmailUniqueAsync(Email))
                {
                    ErrorMessage = "Email уже используется";
                    return;
                }

                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Role = SelectedRole
                };

                bool success = await _authService.RegisterAsync(user, Password);
                if (success)
                {
                    RegistrationSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ErrorMessage = "Ошибка при регистрации. Пожалуйста, попробуйте снова.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при регистрации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Введите имя пользователя";
                CanRegister = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите пароль";
                CanRegister = false;
                return;
            }

            if (Password.Length < 4)
            {
                ErrorMessage = "Пароль должен содержать минимум 4 символа";
                CanRegister = false;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                CanRegister = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Введите email";
                CanRegister = false;
                return;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Введите корректный email";
                CanRegister = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = "Введите имя";
                CanRegister = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Введите фамилию";
                CanRegister = false;
                return;
            }

            ErrorMessage = string.Empty;
            CanRegister = true;
        }
    }
}