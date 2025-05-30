using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

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

        public async Task<(string login, string password)> CreateTeacherAsync(string name, string middleName, string lastName)
        {
            var login = GenerateLogin(name, lastName);
            var password = GeneratePassword();

            var user = new User
            {
                UserName = login,
                Email = $"{login}@sibupk.su",
                Name = name,
                MiddleName = middleName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Teacher");

            return (login, password);
        }

        public async Task<(string login, string password)> CreateStudentAsync(string name, string middleName, string lastName)
        {
            var login = GenerateLogin(name, lastName);
            var password = GeneratePassword();

            var user = new User
            {
                UserName = login,
                Email = $"{login}@sibupk.su",
                Name = name,
                MiddleName = middleName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Student");

            return (login, password);
        }


        private string GenerateLogin(string name, string lastName)
        {
            // Удаляем не-латинские символы, пробелы и спецсимволы
            string clean = $"{Transliterate(lastName)}{Transliterate(name)}";
            return clean.ToLower();
        }
        private string Transliterate(string input)
        {
            Dictionary<char, string> map = new Dictionary<char, string>
            {
                ['а'] = "a",
                ['б'] = "b",
                ['в'] = "v",
                ['г'] = "g",
                ['д'] = "d",
                ['е'] = "e",
                ['ё'] = "e",
                ['ж'] = "zh",
                ['з'] = "z",
                ['и'] = "i",
                ['й'] = "y",
                ['к'] = "k",
                ['л'] = "l",
                ['м'] = "m",
                ['н'] = "n",
                ['о'] = "o",
                ['п'] = "p",
                ['р'] = "r",
                ['с'] = "s",
                ['т'] = "t",
                ['у'] = "u",
                ['ф'] = "f",
                ['х'] = "h",
                ['ц'] = "ts",
                ['ч'] = "ch",
                ['ш'] = "sh",
                ['щ'] = "shch",
                ['ъ'] = "",
                ['ы'] = "y",
                ['ь'] = "",
                ['э'] = "e",
                ['ю'] = "yu",
                ['я'] = "ya"
            };

            var sb = new StringBuilder();

            foreach (char c in input.ToLower())
            {
                if (map.TryGetValue(c, out var latin))
                    sb.Append(latin);
                else if (char.IsLetterOrDigit(c))
                    sb.Append(c); // оставить латинские буквы и цифры
                                  // игнорировать всё остальное
            }

            return sb.ToString();
        }


        private string GeneratePassword()
        {
            return "DefaultPass123"; // Можно использовать генератор случайных паролей
        }
    }
}