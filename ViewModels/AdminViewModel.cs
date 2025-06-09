using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using IntellectFlow.Models;
using System.Linq;
using IntellectFlow.Views;

namespace IntellectFlow.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _dbContext;
        private readonly UserService _userService;

        public AdminViewModel(IntellectFlowDbContext dbContext, UserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;

            Task.Run(async () => await LoadDataAsync());

            AddDisciplineCommand = new RelayCommand(async _ => await AddDisciplineAsync(), _ => CanAddDiscipline());
            AddCourseCommand = new RelayCommand(async _ => await AddCourseAsync(), _ => CanAddCourse());
            AddGroupCommand = new RelayCommand(async _ => await AddGroupAsync(), _ => CanAddGroup());
            DeleteGroupCommand = new RelayCommand(async _ => await DeleteGroupAsync(), _ => CanDeleteGroup());
            DeleteUserCommand = new RelayCommand(async _ => await DeleteUserAsync(), _ => CanDeleteUser());
            AddStudentCommand = new RelayCommand(_ => AddStudent());
            DeleteDisciplineCommand = new RelayCommand(async _ => await DeleteDisciplineAsync(), _ => CanDeleteDiscipline());
            DeleteCourseCommand = new RelayCommand(async _ => await DeleteCourseAsync(), _ => CanDeleteCourse());
        }

        private async Task LoadDataAsync()
        {
            var students = await _dbContext.Students.Include(s => s.User).ToListAsync();
            var teachers = await _dbContext.Teachers.Include(t => t.User).ToListAsync();
            var groups = await _dbContext.Groups.ToListAsync();
            var disciplines = await _dbContext.Disciplines.ToListAsync();
            var courses = await _dbContext.Courses.ToListAsync();

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

                Groups.Clear();
                foreach (var g in groups)
                    Groups.Add(g);
            });
        }

        // Коллекции
        public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();
        public ObservableCollection<Teacher> Teachers { get; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Discipline> Disciplines { get; } = new ObservableCollection<Discipline>();
        public ObservableCollection<Course> Courses { get; } = new ObservableCollection<Course>();
        public ObservableCollection<Group> Groups { get; } = new ObservableCollection<Group>();




        // Выбранные элементы
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
                    DeleteUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

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
                    DeleteUserCommand.RaiseCanExecuteChanged();
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
                    DeleteDisciplineCommand.RaiseCanExecuteChanged();
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
                    DeleteCourseCommand.RaiseCanExecuteChanged();
                }
            }
        }
 
        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    OnPropertyChanged();
                    DeleteGroupCommand.RaiseCanExecuteChanged();
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
        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged();
                    AddGroupCommand.RaiseCanExecuteChanged();
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
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand AddStudentCommand { get; }
        public RelayCommand DeleteDisciplineCommand { get; }
        public RelayCommand DeleteCourseCommand { get; }
        public RelayCommand AddGroupCommand { get; }
        public RelayCommand DeleteGroupCommand { get; }

        // Проверки CanExecute
        private bool CanAddDiscipline() => !string.IsNullOrWhiteSpace(DisciplineName);

        private bool CanAddCourse() => !string.IsNullOrWhiteSpace(CourseName) && SelectedDiscipline != null;

        private bool CanDeleteUser() => SelectedStudent != null || SelectedTeacher != null;

        private bool CanDeleteDiscipline() => SelectedDiscipline != null;

        private bool CanDeleteCourse() => SelectedCourse != null;

        // Методы команд
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
        private void AddStudent()
        {
            var addStudentWindow = new AddStudentWindow(_dbContext);
            if (addStudentWindow.ShowDialog() == true)
            {
                _dbContext.Students.Add(addStudentWindow.NewStudent);
                _dbContext.SaveChanges();
                Students.Add(addStudentWindow.NewStudent);
            }
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

        public async Task DeleteUserAsync()
        {
            string userNameToDelete = null;

            if (SelectedTeacher != null)
                userNameToDelete = SelectedTeacher.User?.UserName;

            else if (SelectedStudent != null)
                userNameToDelete = SelectedStudent.User?.UserName;

            if (string.IsNullOrEmpty(userNameToDelete))
            {
                MessageBox.Show("Пользователь не выбран или у пользователя отсутствует логин");
                return;
            }

            try
            {
                await _userService.DeleteUserAsync(userNameToDelete);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (SelectedTeacher != null)
                    {
                        Teachers.Remove(SelectedTeacher);
                        SelectedTeacher = null;
                    }
                    else if (SelectedStudent != null)
                    {
                        Students.Remove(SelectedStudent);
                        SelectedStudent = null;
                    }
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}");
            }
        }

        public async Task DeleteDisciplineAsync()
        {
            if (!CanDeleteDiscipline()) return;

            var discipline = SelectedDiscipline;

            // Проверяем, есть ли связанные курсы (опционально — запретить удаление, если есть связанные данные)
            var hasCourses = await _dbContext.Courses.AnyAsync(c => c.DisciplineId == discipline.Id);
            if (hasCourses)
            {
                MessageBox.Show("Нельзя удалить дисциплину, так как есть связанные курсы.");
                return;
            }

            _dbContext.Disciplines.Remove(discipline);
            await _dbContext.SaveChangesAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Disciplines.Remove(discipline);
                SelectedDiscipline = null;
            });
        }

        public async Task DeleteCourseAsync()
        {
            if (!CanDeleteCourse()) return;

            var course = SelectedCourse;

            // Здесь можно добавить проверку на связанные данные, если нужно

            _dbContext.Courses.Remove(course);
            await _dbContext.SaveChangesAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Courses.Remove(course);
                SelectedCourse = null;
            });
        }


        // Метод для добавления группы
        private async Task AddGroupAsync()
        {
            if (!CanAddGroup()) return;

            var group = new Group { Name = GroupName.Trim() };

            try
            {
                _dbContext.Groups.Add(group);
                await _dbContext.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Groups.Add(group);
                    GroupName = string.Empty; // Очищаем поле ввода
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении группы: {ex.Message}");
            }
        }
        private bool CanAddGroup() => !string.IsNullOrWhiteSpace(GroupName);

        // Проверка возможности удаления группы
        private bool CanDeleteGroup() => SelectedGroup != null;

        // Метод для удаления группы
        private async Task DeleteGroupAsync()
        {
            if (SelectedGroup == null)
                return;

            // Проверяем, есть ли студенты в группе
            var hasStudents = await _dbContext.StudentGroups
                .AnyAsync(sg => sg.GroupId == SelectedGroup.Id);

            if (hasStudents)
            {
                MessageBox.Show("Нельзя удалить группу, так как в ней есть студенты.");
                return;
            }

            try
            {
                _dbContext.Groups.Remove(SelectedGroup);
                await _dbContext.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Groups.Remove(SelectedGroup);
                    SelectedGroup = null;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении группы: {ex.Message}");
            }
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            AddDisciplineCommand?.RaiseCanExecuteChanged();
            AddCourseCommand?.RaiseCanExecuteChanged();
            DeleteUserCommand?.RaiseCanExecuteChanged();
            DeleteDisciplineCommand?.RaiseCanExecuteChanged();
            DeleteCourseCommand?.RaiseCanExecuteChanged();
            AddGroupCommand?.RaiseCanExecuteChanged();
            DeleteGroupCommand?.RaiseCanExecuteChanged();
        }
    }
}
