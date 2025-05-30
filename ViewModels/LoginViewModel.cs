using CommunityToolkit.Mvvm.Input;
using IntellectFlow.Helpers;
using IntellectFlow.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // 1. Инициализация ServiceProvider
                mainWindow.Initialize(_serviceProvider);

                // 2. Определение основной роли
                var primaryRole = roles.Contains("Admin") ? "Admin" :
                                  roles.Contains("Teacher") ? "Teacher" : "Student";

                // 3. Установка контента по роли
                mainWindow.SetContentForRole(primaryRole);

                // 4. Показ главного окна до закрытия LoginView
                mainWindow.Show();

                // 5. Закрытие окна авторизации
                Application.Current.Windows
                    .OfType<LoginView>()
                    .FirstOrDefault()?
                    .Close();
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
