using IntellectFlow.DataModel;
using IntellectFlow.Models;
using IntellectFlow.Views;
using IntellectFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace IntellectFlow.ViewModels
{
    public class TeacherViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _db;

        public TeacherViewModel(IntellectFlowDbContext db, int teacherId)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            TeacherId = teacherId;

            LoadData();

            AddCourseCommand = new RelayCommand(_ => AddCourse(), _ => CanAddCourse());
            AddStudentToCourseCommand = new RelayCommand(_ => AddStudentToCourse(), _ => SelectedStudentToAdd != null && SelectedCourse != null);
            RemoveStudentFromCourseCommand = new RelayCommand(_ => RemoveStudentFromCourse(), _ => SelectedStudentInCourse != null && SelectedCourse != null);
            RemoveCourseCommand = new RelayCommand(_ => RemoveCourse(), _ => SelectedCourse != null);


        }

        public int TeacherId { get; }

        // Коллекции для привязки
        public ObservableCollection<Discipline> AllDisciplines { get; } = new ObservableCollection<Discipline>();
        public ObservableCollection<Discipline> MyDisciplines { get; } = new ObservableCollection<Discipline>();
        public ObservableCollection<Course> MyCourses { get; } = new ObservableCollection<Course>();
        public ObservableCollection<Student> StudentsInCourse { get; } = new ObservableCollection<Student>();
        public ObservableCollection<Student> AvailableStudents { get; } = new ObservableCollection<Student>();
        public ObservableCollection<Student> AllStudents { get; } = new ObservableCollection<Student>();

        // Выбранные элементы
        private Discipline? _selectedDiscipline;
        public Discipline? SelectedDiscipline
        {
            get => _selectedDiscipline;
            set
            {
                if (_selectedDiscipline != value)
                {
                    _selectedDiscipline = value;
                    OnPropertyChanged(nameof(SelectedDiscipline));
                    UpdateAvailableCourses();
                    (AddCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private Course? _selectedCourse;
        public Course? SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                if (_selectedCourse != value)
                {
                    _selectedCourse = value;
                    OnPropertyChanged(nameof(SelectedCourse));
                    LoadStudentsInCourse();
                    UpdateAvailableStudents(); // обновляем доступных студентов при смене курса
                    (AddStudentToCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (RemoveStudentFromCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (RemoveCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();

                }
            }
        }

        private Student? _selectedStudentInCourse;
        public Student? SelectedStudentInCourse
        {
            get => _selectedStudentInCourse;
            set
            {
                if (_selectedStudentInCourse != value)
                {
                    _selectedStudentInCourse = value;
                    OnPropertyChanged(nameof(SelectedStudentInCourse));
                    (RemoveStudentFromCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private Student? _selectedStudentToAdd;
        public Student? SelectedStudentToAdd
        {
            get => _selectedStudentToAdd;
            set
            {
                if (_selectedStudentToAdd != value)
                {
                    _selectedStudentToAdd = value;
                    OnPropertyChanged(nameof(SelectedStudentToAdd));
                    (AddStudentToCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _courseName = string.Empty;
        public string CourseName
        {
            get => _courseName;
            set
            {
                if (_courseName != value)
                {
                    _courseName = value;
                    OnPropertyChanged(nameof(CourseName));
                    (AddCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _courseDescription = string.Empty;
        public string CourseDescription
        {
            get => _courseDescription;
            set
            {
                if (_courseDescription != value)
                {
                    _courseDescription = value;
                    OnPropertyChanged(nameof(CourseDescription));
                }
            }
        }

        // Команды
        public ICommand AddCourseCommand { get; }
        public ICommand AddStudentToCourseCommand { get; }
        public ICommand RemoveStudentFromCourseCommand { get; }
        public ICommand RemoveCourseCommand { get; }

        private void LoadData()
        {
            AllDisciplines.Clear();
            foreach (var d in _db.Disciplines.OrderBy(d => d.Name))
                AllDisciplines.Add(d);

            UpdateMyDisciplines();

            MyCourses.Clear();
            foreach (var c in _db.Courses.Where(c => c.TeacherId == TeacherId).OrderBy(c => c.Name))
                MyCourses.Add(c);

            AllStudents.Clear();
            var studentsOrdered = _db.Students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.Name)
                .ThenBy(s => s.MiddleName);

            foreach (var student in studentsOrdered)
                AllStudents.Add(student);
        }

        private void UpdateMyDisciplines()
        {
            MyDisciplines.Clear();
            var disciplineIdsWithCourses = _db.Courses
                .Where(c => c.TeacherId == TeacherId)
                .Select(c => c.DisciplineId)
                .Distinct()
                .ToList();

            var disciplines = _db.Disciplines
                .Where(d => disciplineIdsWithCourses.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToList();

            foreach (var d in disciplines)
                MyDisciplines.Add(d);
        }

        private void UpdateAvailableCourses()
        {
            // Можно реализовать, если нужно фильтровать курсы по дисциплине
        }

        private void LoadStudentsInCourse()
        {
            StudentsInCourse.Clear();

            if (SelectedCourse == null)
                return;

            var students = _db.StudentCourses
                .Where(sc => sc.CourseId == SelectedCourse.Id)
                .Select(sc => sc.Student)
                .ToList()
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.Name)
                .ThenBy(s => s.MiddleName)
                .ToList();

            foreach (var student in students)
                StudentsInCourse.Add(student);
        }

        private void UpdateAvailableStudents()
        {
            AvailableStudents.Clear();

            if (SelectedCourse == null)
                return;

            var studentsNotInCourse = _db.Students
                .ToList()
                .Where(s => !s.StudentCourses.Any(sc => sc.CourseId == SelectedCourse.Id))
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.Name)
                .ThenBy(s => s.MiddleName)
                .ToList();

            foreach (var student in studentsNotInCourse)
                AvailableStudents.Add(student);
        }

        private bool CanAddCourse()
        {
            return SelectedDiscipline != null && !string.IsNullOrWhiteSpace(CourseName);
        }

        private void AddCourse()
        {
            if (!CanAddCourse())
                return;

            var course = new Course
            {
                Name = CourseName.Trim(),
                Description = string.IsNullOrWhiteSpace(CourseDescription) ? null : CourseDescription.Trim(),
                DisciplineId = SelectedDiscipline!.Id,
                TeacherId = TeacherId
            };

            _db.Courses.Add(course);
            _db.SaveChanges();

            MyCourses.Add(course);

            // Обновляем список "Мои дисциплины" после добавления курса
            UpdateMyDisciplines();

            CourseName = string.Empty;
            CourseDescription = string.Empty;
            SelectedDiscipline = null;
        }
        // В TeacherViewModel добавим обработчик двойного клика:
        public void OpenCourseDetails()
        {
            if (SelectedCourse == null) return;

            var courseDetailsVM = new CourseDetailsViewModel(_db, SelectedCourse.Id);
            var courseDetailsWindow = new CourseDetailsWindow
            {
                DataContext = courseDetailsVM
            };
            courseDetailsWindow.ShowDialog();
        }

        private void AddStudentToCourse()
        {
            if (SelectedCourse == null || SelectedStudentToAdd == null)
                return;

            var exists = _db.StudentCourses.Any(sc => sc.CourseId == SelectedCourse.Id && sc.StudentId == SelectedStudentToAdd.Id);
            if (exists)
                return;

            var sc = new StudentCourse
            {
                CourseId = SelectedCourse.Id,
                StudentId = SelectedStudentToAdd.Id,
                EnrolledDate = DateTime.UtcNow
            };

            _db.StudentCourses.Add(sc);
            _db.SaveChanges();

            LoadStudentsInCourse();
            UpdateAvailableStudents();

            SelectedStudentToAdd = null;
        }

        private void RemoveStudentFromCourse()
        {
            if (SelectedCourse == null || SelectedStudentInCourse == null)
                return;

            var sc = _db.StudentCourses.FirstOrDefault(x => x.CourseId == SelectedCourse.Id && x.StudentId == SelectedStudentInCourse.Id);
            if (sc != null)
            {
                _db.StudentCourses.Remove(sc);
                _db.SaveChanges();

                LoadStudentsInCourse();
                UpdateAvailableStudents();

                SelectedStudentInCourse = null;
            }
        }
        private void RemoveCourse()
        {
            if (SelectedCourse == null)
                return;

            // Удаляем связанные записи, если есть (например, StudentCourses)
            var studentCourses = _db.StudentCourses.Where(sc => sc.CourseId == SelectedCourse.Id).ToList();
            if (studentCourses.Any())
            {
                _db.StudentCourses.RemoveRange(studentCourses);
            }

            _db.Courses.Remove(SelectedCourse);
            _db.SaveChanges();

            MyCourses.Remove(SelectedCourse);

            // Обновляем "Мои дисциплины" (если в результате удаления курса дисциплина стала неактивна)
            UpdateMyDisciplines();

            // Очистим выбор
            SelectedCourse = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
