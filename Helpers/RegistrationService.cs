using IntellectFlow.DataModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class RegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleService _roleService;

        public RegistrationService(UserManager<User> userManager, RoleService roleService)
        {
            _userManager = userManager;
            _roleService = roleService;
        }

        public async Task<bool> RegisterUserAsync(string email, string password, string roleName)
        {
            // Создаем пользователя
            var user = new User { UserName = email, Email = email };
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                // Можно логировать ошибки
                return false;
            }

            // Создаем роли, если их еще нет
            await _roleService.EnsureRolesCreated();

            // Назначаем роль пользователю
            if (new[] { "Admin", "Teacher", "Student" }.Contains(roleName))
            {
                var roleAssignResult = await _roleService.AssignRoleToUser(user, roleName);
                if (!roleAssignResult.Succeeded)
                    return false;
            }
            else
            {
                // Если роль невалидна — назначаем Student по умолчанию
                await _roleService.AssignRoleToUser(user, "Student");
            }

            return true;
        }
    }

}
