using System.ComponentModel;
using System.Runtime.CompilerServices;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;

    public object CurrentView => _navigationService.CurrentView;

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_navigationService.CurrentView))
                OnPropertyChanged(nameof(CurrentView));
        };
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
