using IntellectFlow.DataModel;
using IntellectFlow.Models; // где DbContext
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class RegistrationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleService _roleService;
        private readonly IntellectFlowDbContext _dbContext;

        public RegistrationService(UserManager<User> userManager, RoleService roleService, IntellectFlowDbContext dbContext)
        {
            _userManager = userManager;
            _roleService = roleService;
            _dbContext = dbContext;
        }

        public async Task<bool> RegisterUserAsync(string email, string password, string roleName, string name, string middleName, string lastName)
        {
            // Создаем пользователя Identity
            var user = new User
            {
                UserName = email,
                Email = email,
                Name = name,
                MiddleName = middleName,
                LastName = lastName
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                // Можно логировать ошибки
                return false;
            }

            await _roleService.EnsureRolesCreated();

            // Назначаем роль
            var validRoles = new[] { "Admin", "Teacher", "Student" };
            if (!validRoles.Contains(roleName))
                roleName = "Student"; // по умолчанию студент

            var roleAssignResult = await _roleService.AssignRoleToUser(user, roleName);
            if (!roleAssignResult.Succeeded)
                return false;

            // Создаем запись в таблице Student или Teacher, в зависимости от роли
            if (roleName == "Student")
            {
                var student = new Student
                {
                    UserId = user.Id,
                    Name = name,
                    MiddleName = middleName,
                    LastName = lastName
                };
                _dbContext.Students.Add(student);
            }
            else if (roleName == "Teacher")
            {
                var teacher = new Teacher
                {
                    UserId = user.Id,
                    Name = name,
                    MiddleName = middleName,
                    LastName = lastName
                };
                _dbContext.Teachers.Add(teacher);
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
