using CommunityToolkit.Mvvm.Input;
using IntellectFlow.Helpers;
using IntellectFlow.Views;
using IntellectFlow;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;

    public ICommand LoginCommand { get; }
    public string Email { get; set; }

    public LoginViewModel(
        AuthService authService,
        INavigationService navigationService,
        IServiceProvider serviceProvider)
    {
        _authService = authService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;

        LoginCommand = new RelayCommand<string>(ExecuteLogin);
    }

    private async void ExecuteLogin(string password)
    {
        try
        {
            var roles = await _authService.Login(Email, password);
            if (roles != null)
            {
                Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // 💡 Обязательно вызываем Initialize
                mainWindow.Initialize(_serviceProvider);

                var primaryRole = roles.Contains("Admin") ? "Admin" :
                                  roles.Contains("Teacher") ? "Teacher" :
                                  "Student";

                mainWindow.SetContentForRole(primaryRole);
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Неверный email или пароль");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка входа: {ex.Message}");
        }
    }



    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}