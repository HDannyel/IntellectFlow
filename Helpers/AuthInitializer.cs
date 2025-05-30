using IntellectFlow.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IntellectFlow.Helpers
{
    public static class AuthInitializer
    {
        public static async Task EnsureAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123";
            const string adminRole = "Admin";

            // Создаем роль, если не существует
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(adminRole));
            }

            // Создаем пользователя, если не существует
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
                else
                {
                    throw new Exception("Ошибка создания пользователя: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
