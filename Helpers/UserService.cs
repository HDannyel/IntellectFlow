using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace IntellectFlow.Helpers
{
    public class UserService
    {
        private readonly IntellectFlowDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserService(IntellectFlowDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Пример метода для создания преподавателя
        public (string login, string password) CreateTeacher(string name, string middleName, string lastName)
        {
            var login = GenerateLogin(name, lastName);
            var password = GeneratePassword();

            // Здесь можно реализовать логику создания User + роли Teacher

            return (login, password);
        }

        // Пример метода для создания студента
        public (string login, string password) CreateStudent(string name, string middleName, string lastName)
        {
            var login = GenerateLogin(name, lastName);
            var password = GeneratePassword();

            // Здесь можно реализовать логику создания User + роли Student

            return (login, password);
        }

        private string GenerateLogin(string name, string lastName)
        {
            return $"{name.ToLower()}.{lastName.ToLower()}";
        }

        private string GeneratePassword()
        {
            return "DefaultPass123"; // Можно использовать генератор случайных паролей
        }
    }
}