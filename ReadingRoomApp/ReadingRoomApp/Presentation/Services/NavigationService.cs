using System;
using System.Collections.Generic;

namespace ReadingRoomApp.Presentation.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _navigationStack = new Stack<object>();
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                ViewChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<object> ViewChanged;

        public void NavigateTo<T>(T viewModel)
        {
            if (_currentView != null)
            {
                _navigationStack.Push(_currentView);
            }

            CurrentView = viewModel;
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count > 0)
            {
                CurrentView = _navigationStack.Pop();
            }
        }
    }
}