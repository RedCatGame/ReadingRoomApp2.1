using System;

namespace ReadingRoomApp.Presentation.Services
{
    public interface INavigationService
    {
        object CurrentView { get; }
        event EventHandler<object> ViewChanged;

        void NavigateTo<T>(T viewModel);
        void NavigateBack();
    }
}