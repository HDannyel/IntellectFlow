using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Views;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using IntellectFlow.Models;

public class AuthService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly INavigationService _navigationService;
    private readonly IUserContext _userContext;
    private readonly IntellectFlowDbContext _dbContext;

    public AuthService(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        INavigationService navigationService,
        IUserContext userContext,
        IntellectFlowDbContext dbContext) // <-- добавили
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _navigationService = navigationService;
        _userContext = userContext;
        _dbContext = dbContext; // <-- сохранили
    }


    public async Task<IList<string>> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        int? teacherId = null;
        int? studentId = null;

        if (roles.Contains("Teacher"))
        {
            teacherId = await _dbContext.Teachers
                .Where(t => t.UserId == user.Id)
                .Select(t => (int?)t.Id)
                .FirstOrDefaultAsync();
        }
        else if (roles.Contains("Student"))
        {
            studentId = await _dbContext.Students
                .Where(s => s.UserId == user.Id)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync();
        }

        _userContext.SetUser(user.Id, user.UserName, roles.FirstOrDefault() ?? string.Empty, teacherId, studentId);

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
