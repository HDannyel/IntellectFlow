using System;
using System.ComponentModel;

public interface INavigationService : INotifyPropertyChanged
{
    object CurrentView { get; }
    void NavigateTo<TView>() where TView : class;
}
