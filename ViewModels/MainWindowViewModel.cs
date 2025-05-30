using IntellectFlow.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly INavigationService _navigationService;

    public object CurrentPage => _navigationService.CurrentView;

    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        // Опционально: подписка на обновления
        if (_navigationService is NavigationService nav)
        {
            nav.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(nav.CurrentView))
                    OnPropertyChanged(nameof(CurrentPage));
            };
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}