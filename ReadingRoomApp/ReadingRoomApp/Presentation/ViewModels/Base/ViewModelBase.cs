using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ReadingRoomApp.Common.Logging;

namespace ReadingRoomApp.Presentation.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _errorMessage;

        protected ILogger Logger => App.Logger;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected async Task ExecuteAsync(Func<Task> action, string errorMessage = "Произошла ошибка")
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                await action();
            }
            catch (Exception ex)
            {
                Logger?.LogError($"{errorMessage}: {ex.Message}");
                ErrorMessage = $"{errorMessage}: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string errorMessage = "Произошла ошибка")
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                return await action();
            }
            catch (Exception ex)
            {
                Logger?.LogError($"{errorMessage}: {ex.Message}");
                ErrorMessage = $"{errorMessage}: {ex.Message}";
                return default;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}