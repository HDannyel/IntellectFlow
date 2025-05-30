using Microsoft.AspNetCore.Identity;
using IntellectFlow.DataModel;

namespace IntellectFlow.Helpers
{
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

        public async Task AssignRoleToUser(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
    }
}