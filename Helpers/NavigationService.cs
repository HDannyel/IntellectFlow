using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;

public class NavigationService : INavigationService, INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    private object _currentView;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
    }

    public void NavigateTo<TView>() where TView : class
    {
        var view = _serviceProvider.GetService<TView>();
        if (view == null)
            throw new InvalidOperationException($"View of type {typeof(TView).Name} is not registered");

        CurrentView = view;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
