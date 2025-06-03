using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Views;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

public class AuthService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly INavigationService _navigationService;
    private readonly IUserContext _userContext;

    public AuthService(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        INavigationService navigationService,
        IUserContext userContext)  // добавляем
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _navigationService = navigationService;
        _userContext = userContext; // сохраняем
    }

    public async Task<IList<string>> Login(string email, string password)
    {
        Debug.WriteLine($"Попытка входа: email='{email}', пароль='{password}'");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            Debug.WriteLine($"Пользователь с email '{email}' не найден");
            return null;
        }

        Debug.WriteLine($"Пользователь найден: {user.UserName}");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        Debug.WriteLine($"Пароль корректен: {isPasswordValid}");

        if (!isPasswordValid)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        Debug.WriteLine($"Роли пользователя: {string.Join(", ", roles)}");

        // Записываем в UserContext
        _userContext.SetUser(user.Id, user.UserName, roles.FirstOrDefault() ?? string.Empty);

        return roles;
    }



    public async Task ShowMainWindow()
    {
        var role = _userContext.UserRole;

        switch (role)
        {
            case "Admin":
                _navigationService.NavigateTo<AdminView>();
                break;
            case "Teacher":
                _navigationService.NavigateTo<TeacherView>();
                break;
            default:
                _navigationService.NavigateTo<StudentView>();
                break;
        }
    }

}