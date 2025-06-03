using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.ViewModels
{
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _context = new();
        private readonly int _teacherId;

        // ───────────────────────────────────────────  Коллекции
        public ObservableCollection<Discipline> Disciplines { get; } = new();
        public ObservableCollection<Course> Courses { get; } = new();
        public ObservableCollection<Student> AvailableStudents { get; } = new();
        public ObservableCollection<Student> StudentsInCourse { get; } = new();

        // ───────────────────────────────────────────  Выборы
        private Discipline _selectedDiscipline;
        public Discipline SelectedDiscipline
        {
            get => _selectedDiscipline;
            set
            {
                if (_selectedDiscipline != value)
                {
                    _selectedDiscipline = value;
                    OnPropertyChanged();
                    RefreshCourses();
                }
            }
        }

        private Course _selectedCourse;
        public Course SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                if (_selectedCourse != value)
                {
                    _selectedCourse = value;
                    OnPropertyChanged();
                    RefreshStudentsInCourse();
                }
            }
        }

        private Student _selectedStudentToAdd;
        public Student SelectedStudentToAdd
        {
            get => _selectedStudentToAdd;
            set { _selectedStudentToAdd = value; OnPropertyChanged(); }
        }

        private Student _selectedStudentInCourse;
        public Student SelectedStudentInCourse
        {
            get => _selectedStudentInCourse;
            set { _selectedStudentInCourse = value; OnPropertyChanged(); }
        }

        // ───────────────────────────────────────────  Поля ввода
        private string _disciplineName;
        public string DisciplineName
        {
            get => _disciplineName;
            set { _disciplineName = value; OnPropertyChanged(); }
        }

        private string _disciplineDescription;
        public string DisciplineDescription
        {
            get => _disciplineDescription;
            set { _disciplineDescription = value; OnPropertyChanged(); }
        }

        private string _courseName;
        public string CourseName
        {
            get => _courseName;
            set { _courseName = value; OnPropertyChanged(); }
        }

        private string _courseDescription;
        public string CourseDescription
        {
            get => _courseDescription;
            set { _courseDescription = value; OnPropertyChanged(); }
        }

        // ───────────────────────────────────────────  Команды
        public ICommand AddDisciplineCommand { get; }
        public ICommand DeleteDisciplineCommand { get; }
        public ICommand AddCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }
        public ICommand AddStudentToCourseCommand { get; }
        public ICommand RemoveStudentFromCourseCommand { get; }

        // ───────────────────────────────────────────  ctor
        public TeacherViewModel(int teacherId)
        {
            _teacherId = teacherId;

            AddDisciplineCommand = new RelayCommand(_ => AddDiscipline(), _ => !string.IsNullOrWhiteSpace(DisciplineName));
            DeleteDisciplineCommand = new RelayCommand(_ => DeleteDiscipline(), _ => SelectedDiscipline != null);
            AddCourseCommand = new RelayCommand(_ => AddCourse(), _ => SelectedDiscipline != null && !string.IsNullOrWhiteSpace(CourseName));
            DeleteCourseCommand = new RelayCommand(_ => DeleteCourse(), _ => SelectedCourse != null);
            AddStudentToCourseCommand = new RelayCommand(_ => AddStudent(), _ => SelectedCourse != null && SelectedStudentToAdd != null);
            RemoveStudentFromCourseCommand = new RelayCommand(_ => RemoveStudent(), _ => SelectedCourse != null && SelectedStudentInCourse != null);

            LoadInitialData();
        }

        // ───────────────────────────────────────────  Загрузка
        private void LoadInitialData()
        {
            Disciplines.Clear();
            foreach (var d in _context.Disciplines.ToList())
                Disciplines.Add(d);

            RefreshCourses();
        }

        private void RefreshCourses()
        {
            Courses.Clear();

            if (SelectedDiscipline == null)
                return;

            var list = _context.Courses
                               .Include(c => c.Discipline)
                               .Where(c => c.TeacherId == _teacherId && c.DisciplineId == SelectedDiscipline.Id)
                               .ToList();

            foreach (var c in list)
                Courses.Add(c);

            SelectedCourse = null;
        }

        private void RefreshStudentsInCourse()
        {
            StudentsInCourse.Clear();
            AvailableStudents.Clear();

            if (SelectedCourse == null) return;

            var inCourse = _context.StudentCourses
                                   .Where(sc => sc.CourseId == SelectedCourse.Id)
                                   .Select(sc => sc.Student)
                                   .Include(s => s.User)
                                   .ToList();

            var notInCourse = _context.Students
                                      .Include(s => s.User)
                                      .Where(s => !_context.StudentCourses
                                          .Any(sc => sc.StudentId == s.Id && sc.CourseId == SelectedCourse.Id))
                                      .ToList();

            foreach (var s in inCourse) StudentsInCourse.Add(s);
            foreach (var s in notInCourse) AvailableStudents.Add(s);
        }

        // ───────────────────────────────────────────  Методы-команды
        private void AddDiscipline()
        {
            var d = new Discipline
            {
                Name = DisciplineName.Trim(),
                Description = string.IsNullOrWhiteSpace(DisciplineDescription) ? null : DisciplineDescription.Trim()
            };
            _context.Disciplines.Add(d);
            _context.SaveChanges();

            Disciplines.Add(d);
            DisciplineName = DisciplineDescription = string.Empty;
        }

        private void DeleteDiscipline()
        {
            if (SelectedDiscipline == null) return;

            // удаляем все курсы данной дисциплины преподавателя
            var courses = _context.Courses
                                  .Where(c => c.TeacherId == _teacherId && c.DisciplineId == SelectedDiscipline.Id)
                                  .ToList();

            _context.Courses.RemoveRange(courses);
            _context.Disciplines.Remove(SelectedDiscipline);
            _context.SaveChanges();

            Disciplines.Remove(SelectedDiscipline);
            SelectedDiscipline = null;
            RefreshCourses();
        }

        private void AddCourse()
        {
            var c = new Course
            {
                Name = CourseName.Trim(),
                Description = string.IsNullOrWhiteSpace(CourseDescription) ? null : CourseDescription.Trim(),
                DisciplineId = SelectedDiscipline.Id,
                TeacherId = _teacherId
            };
            _context.Courses.Add(c);
            _context.SaveChanges();

            Courses.Add(c);
            CourseName = CourseDescription = string.Empty;
        }

        private void DeleteCourse()
        {
            if (SelectedCourse == null) return;

            _context.Courses.Remove(SelectedCourse);
            _context.SaveChanges();

            Courses.Remove(SelectedCourse);
            SelectedCourse = null;
            StudentsInCourse.Clear();
            RefreshStudentsInCourse();
        }

        private void AddStudent()
        {
            if (SelectedCourse == null || SelectedStudentToAdd == null) return;

            var entry = new StudentCourse
            {
                CourseId = SelectedCourse.Id,
                StudentId = SelectedStudentToAdd.Id,
                EnrolledDate = DateTime.UtcNow
            };
            _context.StudentCourses.Add(entry);
            _context.SaveChanges();

            AvailableStudents.Remove(SelectedStudentToAdd);
            StudentsInCourse.Add(SelectedStudentToAdd);
            SelectedStudentToAdd = null;
        }

        private void RemoveStudent()
        {
            if (SelectedCourse == null || SelectedStudentInCourse == null) return;

            var entry = _context.StudentCourses
                                .FirstOrDefault(sc => sc.CourseId == SelectedCourse.Id &&
                                                      sc.StudentId == SelectedStudentInCourse.Id);

            if (entry != null)
            {
                _context.StudentCourses.Remove(entry);
                _context.SaveChanges();
            }

            StudentsInCourse.Remove(SelectedStudentInCourse);
            AvailableStudents.Add(SelectedStudentInCourse);
            SelectedStudentInCourse = null;
        }

        // ───────────────────────────────────────────  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
