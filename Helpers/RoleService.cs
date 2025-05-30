using IntellectFlow.DataModel;
using Microsoft.AspNetCore.Identity;

public class RoleService
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleService(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task EnsureRolesCreated()
    {
        foreach (var roleName in new[] { "Admin", "Teacher", "Student" })
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }
    }

    public async Task<IdentityResult> AssignRoleToUser(User user, string roleName)
    {
        return await _userManager.AddToRoleAsync(user, roleName);
    }
}
