using CommunityToolkit.Mvvm.Input;
using IntellectFlow.Helpers;
using IntellectFlow.Views;
using IntellectFlow.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using IntellectFlow.Models;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IntellectFlowDbContext _dbContext;
    private readonly IUserContext _userContext;

    public ICommand LoginCommand { get; }
    public string Email { get; set; }

    public LoginViewModel(
        AuthService authService,
        INavigationService navigationService,
        IServiceProvider serviceProvider,
        IntellectFlowDbContext dbContext,
        IUserContext userContext)
    {
        _authService = authService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
        _userContext = userContext;

        LoginCommand = new RelayCommand<string>(ExecuteLogin);
    }

    private async void ExecuteLogin(string password)
    {
        try
        {
            var roles = await _authService.Login(Email, password);
            if (roles != null)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);
                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден");
                    return;
                }

                Teacher? teacher = null;
                Student? student = null;

                if (roles.Contains("Teacher"))
                {
                    teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                    if (teacher == null)
                    {
                        teacher = new Teacher
                        {
                            UserId = user.Id,
                            Name = user.Name,
                            MiddleName = user.MiddleName ?? string.Empty,
                            LastName = user.LastName
                        };
                        _dbContext.Teachers.Add(teacher);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                else if (roles.Contains("Student"))
                {
                    student = await _dbContext.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (student == null)
                    {
                        student = new Student
                        {
                            UserId = user.Id,
                            Name = user.Name,
                            MiddleName = user.MiddleName ?? string.Empty,
                            LastName = user.LastName
                        };
                        _dbContext.Students.Add(student);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                var primaryRole = roles.Contains("Admin") ? "Admin" :
                                  roles.Contains("Teacher") ? "Teacher" : "Student";

                _userContext.SetUser(user.Id, user.UserName, primaryRole, teacher?.Id, student?.Id);

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Initialize(_serviceProvider);
                mainWindow.SetContentForRole(primaryRole);
                mainWindow.Show();

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


    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
