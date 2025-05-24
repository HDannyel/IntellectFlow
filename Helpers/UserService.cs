using IntellectFlow.DataModel;
using IntellectFlow.Models;

public class UserService
{
    private readonly IntellectFlowDbContext _context;

    public UserService(IntellectFlowDbContext context)
    {
        _context = context;
    }

    public (string login, string password) CreateTeacher(string name, string midleName, string lastName)
    {
        // Создание логина и пароля, сохранение в базу и т.п.
        string login = GenerateLogin(name, midleName, lastName);
        string password = GeneratePassword();

        // Добавление преподавателя в базу
        var teacher = new Teacher
        {
            Name = name,
            MidleName = midleName,
            LastName = lastName,
            Login = login,
            Password = password
        };
        _context.Teachers.Add(teacher);
        _context.SaveChanges();

        return (login, password);
    }

    public (string login, string password) CreateStudent(string name, string midleName, string lastName)
    {
        string login = GenerateLogin(name, midleName, lastName);
        string password = GeneratePassword();

        var student = new Student
        {
            Name = name,
            MidleName = midleName,
            LastName = lastName,
            Login = login,
            Password = password
        };
        _context.Students.Add(student);
        _context.SaveChanges();

        return (login, password);
    }

    private string GenerateLogin(string name, string midleName, string lastName)
    {
        // Пример простой генерации логина
        return $"{name[0]}{midleName[0]}{lastName}".ToLower();
    }

    private string GeneratePassword()
    {
        // Пример простой генерации пароля
        return "12345"; // замени на более надежный метод
    }
}
