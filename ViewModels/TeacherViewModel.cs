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
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _context = new();
        private readonly int _teacherId; // Предположим, что ID известен

        public ObservableCollection<Course> Courses { get; set; }
        public Course SelectedCourse { get; set; }

        public ObservableCollection<Student> AvailableStudents { get; set; } = new();
        public Student SelectedStudentToAdd { get; set; }

        private string _courseName = "";
        private string _courseDescription = "";

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

        public ICommand AddCourseCommand { get; }
        public ICommand AddStudentToCourseCommand { get; }

        public TeacherViewModel(int teacherId)
        {
            _teacherId = teacherId;
            LoadData();

            AddCourseCommand = new RelayCommand(OnAddCourse);
            AddStudentToCourseCommand = new RelayCommand(OnAddStudentToCourse);
        }

        private void LoadData()
        {
            Courses = new ObservableCollection<Course>(
                _context.Courses
                    .Include(c => c.Discipline)
                    .Where(c => c.TeacherId == _teacherId)
                    .ToList()
            );

            AvailableStudents = new ObservableCollection<Student>();

            if (SelectedCourse != null)
            {
                var courseId = SelectedCourse.Id;

                AvailableStudents = new ObservableCollection<Student>(
                    _context.Students
                        .Where(s => !_context.StudentCourses
                            .Any(sc => sc.CourseId == courseId && sc.StudentId == s.Id))
                        .ToList()
                );
            }
        }

        private void OnAddCourse(object obj)
        {
            if (string.IsNullOrWhiteSpace(CourseName)) return;

            var course = new Course
            {
                Name = CourseName.Trim(),
                Description = string.IsNullOrWhiteSpace(CourseDescription) ? null : CourseDescription.Trim(),
                TeacherId = _teacherId,
                DisciplineId = 1 // Пример: по умолчанию первая дисциплина
            };

            _context.Courses.Add(course);
            _context.SaveChanges();
            Courses.Add(course);

            CourseName = "";
            CourseDescription = "";
        }

        private void OnAddStudentToCourse(object obj)
        {
            if (SelectedCourse == null || SelectedStudentToAdd == null) return;

            var studentCourse = new StudentCourse
            {
                CourseId = SelectedCourse.Id,
                StudentId = SelectedStudentToAdd.Id,
                EnrolledDate = DateTime.UtcNow
            };

            _context.StudentCourses.Add(studentCourse);
            _context.SaveChanges();

            AvailableStudents.Remove(SelectedStudentToAdd);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}