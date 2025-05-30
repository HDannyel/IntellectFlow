using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _context = new();

        // Свойства для дисциплин
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public Discipline SelectedDiscipline { get; set; }

        // Свойства для курсов
        public ObservableCollection<Course> Courses { get; set; }
        public Course SelectedCourse { get; set; }

        // Свойства для пользователей
        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }

        public ObservableCollection<Teacher> Teachers { get; set; }
        public Teacher SelectedTeacher { get; set; }

        // Поля для создания
        private string _disciplineName = "";
        private string _disciplineDescription = "";
        private string _courseName = "";
        private string _courseDescription = "";

        private string _studentName = "";
        private string _studentLogin = "";
        private string _studentPassword = "";

        private string _teacherName = "";
        private string _teacherLogin = "";
        private string _teacherPassword = "";

        public string DisciplineName
        {
            get => _disciplineName;
            set { _disciplineName = value; OnPropertyChanged(); }
        }

        public string DisciplineDescription
        {
            get => _disciplineDescription;
            set { _disciplineDescription = value; OnPropertyChanged(); }
        }

        public string CourseName
        {
            get => _courseName;
            set { _courseName = value; OnPropertyChanged(); }
        }

        public string CourseDescription
        {
            get => _courseDescription;
            set { _courseDescription = value; OnPropertyChanged(); }
        }

        public string StudentName
        {
            get => _studentName;
            set { _studentName = value; OnPropertyChanged(); }
        }

        public string StudentLogin
        {
            get => _studentLogin;
            set { _studentLogin = value; OnPropertyChanged(); }
        }

        public string StudentPassword
        {
            get => _studentPassword;
            set { _studentPassword = value; OnPropertyChanged(); }
        }

        public string TeacherName
        {
            get => _teacherName;
            set { _teacherName = value; OnPropertyChanged(); }
        }

        public string TeacherLogin
        {
            get => _teacherLogin;
            set { _teacherLogin = value; OnPropertyChanged(); }
        }

        public string TeacherPassword
        {
            get => _teacherPassword;
            set { _teacherPassword = value; OnPropertyChanged(); }
        }

        public ICommand AddDisciplineCommand { get; }
        public ICommand AddCourseCommand { get; }
        public ICommand AddStudentCommand { get; }
        public ICommand AddTeacherCommand { get; }

        public AdminViewModel()
        {
            LoadData();

            AddDisciplineCommand = new RelayCommand(OnAddDiscipline);
            AddCourseCommand = new RelayCommand(OnAddCourse);
            AddStudentCommand = new RelayCommand(OnAddStudent);
            AddTeacherCommand = new RelayCommand(OnAddTeacher);
        }

        private void LoadData()
        {
            Disciplines = new ObservableCollection<Discipline>(_context.Disciplines.ToList());
            Courses = new ObservableCollection<Course>(_context.Courses.Include(c => c.Discipline).ToList());

            Students = new ObservableCollection<Student>(_context.Students.ToList());
            Teachers = new ObservableCollection<Teacher>(_context.Teachers.ToList());
        }

        private void OnAddDiscipline(object obj)
        {
            if (string.IsNullOrWhiteSpace(DisciplineName)) return;

            var discipline = new Discipline
            {
                Name = DisciplineName.Trim(),
                Description = string.IsNullOrWhiteSpace(DisciplineDescription) ? null : DisciplineDescription.Trim()
            };

            _context.Disciplines.Add(discipline);
            _context.SaveChanges();
            Disciplines.Add(discipline);

            DisciplineName = "";
            DisciplineDescription = "";
        }

        private void OnAddCourse(object obj)
        {
            if (SelectedDiscipline == null || string.IsNullOrWhiteSpace(CourseName)) return;

            var course = new Course
            {
                Name = CourseName.Trim(),
                Description = string.IsNullOrWhiteSpace(CourseDescription) ? null : CourseDescription.Trim(),
                DisciplineId = SelectedDiscipline.Id,
                TeacherId = 1 // Пример: текущий админ создаёт курс
            };

            _context.Courses.Add(course);
            _context.SaveChanges();
            Courses.Add(course);

            CourseName = "";
            CourseDescription = "";
        }

        private void OnAddStudent(object obj)
        {
            if (string.IsNullOrWhiteSpace(StudentLogin) || string.IsNullOrWhiteSpace(StudentName)) return;

            var student = new Student
            {
                Name = StudentName.Trim(),
                Login = StudentLogin.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(StudentPassword.Trim()) // Хешируем пароль
            };

            _context.Students.Add(student);
            _context.SaveChanges();
            Students.Add(student);

            StudentName = "";
            StudentLogin = "";
            StudentPassword = "";
        }

        private void OnAddTeacher(object obj)
        {
            if (string.IsNullOrWhiteSpace(TeacherLogin) || string.IsNullOrWhiteSpace(TeacherName)) return;

            var teacher = new Teacher
            {
                Name = TeacherName.Trim(),
                Login = TeacherLogin.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(TeacherPassword.Trim())
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            Teachers.Add(teacher);

            TeacherName = "";
            TeacherLogin = "";
            TeacherPassword = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}