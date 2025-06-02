using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

            // Создаём запись в таблице Teacher
            var teacher = new Teacher
            {
                UserId = user.Id,
                Name = name,
                MiddleName = middleName,
                LastName = lastName
            };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

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

            // Создаём запись в таблице Student
            var student = new Student
            {
                UserId = user.Id,
                Name = name,
                MiddleName = middleName,
                LastName = lastName
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return (login, password);
        }

        private string GenerateLogin(string name, string lastName)
        {
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
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private string GeneratePassword()
        {
            return "DefaultPass123"; // Можно заменить генератором паролей
        }


        public async Task DeleteUserAsync(string login)
        {
            var user = await _userManager.FindByNameAsync(login);
            if (user == null)
                throw new Exception("Пользователь не найден");

            // Удаляем связанные сущности, если нужны (например, Teacher/Student)
            var teacher = _context.Teachers.FirstOrDefault(t => t.UserId == user.Id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            var student = _context.Students.FirstOrDefault(s => s.UserId == user.Id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _context.SaveChangesAsync();
        }

        // Удаление курса по id
        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new Exception("Курс не найден");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        // Удаление дисциплины по id
        public async Task DeleteDisciplineAsync(int disciplineId)
        {
            var discipline = await _context.Disciplines.FindAsync(disciplineId);
            if (discipline == null)
                throw new Exception("Дисциплина не найдена");

            _context.Disciplines.Remove(discipline);
            await _context.SaveChangesAsync();
        }

    }
}
