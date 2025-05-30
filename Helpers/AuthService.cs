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

    public AuthService(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        INavigationService navigationService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _navigationService = navigationService;
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

        return roles;
    }



    public async Task ShowMainWindow()
    {
        var user = await _signInManager.UserManager.GetUserAsync(_signInManager.Context.User);
        if (user == null) return;

        var role = await _userManager.GetRolesAsync(user);

        switch (role.FirstOrDefault())
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