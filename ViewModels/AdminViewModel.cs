using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;


namespace IntellectFlow.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _dbContext;

        public AdminViewModel(IntellectFlowDbContext dbContext)
        {
            _dbContext = dbContext;
            // Запускаем асинхронную загрузку, но в конструкторе нельзя использовать await напрямую
            // Поэтому запускаем в фоне:
            Task.Run(async () => await LoadDataAsync());

            AddDisciplineCommand = new RelayCommand(async _ => await AddDisciplineAsync(), _ => CanAddDiscipline());
            AddCourseCommand = new RelayCommand(async _ => await AddCourseAsync(), _ => CanAddCourse());
        }

        private async Task LoadDataAsync()
        {
            var students = await _dbContext.Students.Include(s => s.User).ToListAsync();
            var teachers = await _dbContext.Teachers.Include(t => t.User).ToListAsync();

            var disciplines = await _dbContext.Disciplines.ToListAsync();
            var courses = await _dbContext.Courses.ToListAsync();

            // Обновляем коллекции в UI потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                Students.Clear();
                foreach (var s in students)
                    Students.Add(s);

                Teachers.Clear();
                foreach (var t in teachers)
                    Teachers.Add(t);

                Disciplines.Clear();
                foreach (var d in disciplines)
                    Disciplines.Add(d);

                Courses.Clear();
                foreach (var c in courses)
                    Courses.Add(c);
            });

        }

        // Коллекции
        public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();
        public ObservableCollection<Teacher> Teachers { get; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Discipline> Disciplines { get; } = new ObservableCollection<Discipline>();
        public ObservableCollection<Course> Courses { get; } = new ObservableCollection<Course>();

        // Выбранные элементы
        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    OnPropertyChanged();
                }
            }
        }

        private Teacher _selectedTeacher;
        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                if (_selectedTeacher != value)
                {
                    _selectedTeacher = value;
                    OnPropertyChanged();
                }
            }
        }

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
                    AddCourseCommand.RaiseCanExecuteChanged();
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
                }
            }
        }

        // Поля для ввода
        private string _disciplineName;
        public string DisciplineName
        {
            get => _disciplineName;
            set
            {
                if (_disciplineName != value)
                {
                    _disciplineName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _disciplineDescription;
        public string DisciplineDescription
        {
            get => _disciplineDescription;
            set
            {
                if (_disciplineDescription != value)
                {
                    _disciplineDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _courseName;
        public string CourseName
        {
            get => _courseName;
            set
            {
                if (_courseName != value)
                {
                    _courseName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _courseDescription;
        public string CourseDescription
        {
            get => _courseDescription;
            set
            {
                if (_courseDescription != value)
                {
                    _courseDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команды
        public RelayCommand AddDisciplineCommand { get; }
        public RelayCommand AddCourseCommand { get; }

        private bool CanAddDiscipline() => !string.IsNullOrWhiteSpace(DisciplineName);

        private bool CanAddCourse() => !string.IsNullOrWhiteSpace(CourseName) && SelectedDiscipline != null;

        public async Task AddDisciplineAsync()
        {
            if (!CanAddDiscipline()) return;

            var discipline = new Discipline
            {
                Name = DisciplineName,
                Description = DisciplineDescription
            };

            _dbContext.Disciplines.Add(discipline);
            await _dbContext.SaveChangesAsync();

            Disciplines.Add(discipline);

            DisciplineName = string.Empty;
            DisciplineDescription = string.Empty;
        }

        public async Task AddCourseAsync()
        {
            if (!CanAddCourse()) return;

            var course = new Course
            {
                Name = CourseName,
                Description = CourseDescription,
                DisciplineId = SelectedDiscipline.Id
            };

            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();

            Courses.Add(course);

            CourseName = string.Empty;
            CourseDescription = string.Empty;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            AddDisciplineCommand?.RaiseCanExecuteChanged();
            AddCourseCommand?.RaiseCanExecuteChanged();
        }
    }
}
