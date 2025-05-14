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
    public class AdminTeacherViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _context = new();

        // Свойства для дисциплин
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public Discipline SelectedDiscipline { get; set; }

        private string _disciplineName = "";
        private string _disciplineDescription = "";

        // Свойства для курсов
        public ObservableCollection<Course> Courses { get; set; }
        public Course SelectedCourse { get; set; }

        private string _courseName = "";
        private string _courseDescription = "";

        public string DisciplineName
        {
            get => _disciplineName;
            set
            {
                _disciplineName = value;
                OnPropertyChanged();
            }
        }

        public string DisciplineDescription
        {
            get => _disciplineDescription;
            set
            {
                _disciplineDescription = value;
                OnPropertyChanged();
            }
        }

        public string CourseName
        {
            get => _courseName;
            set
            {
                _courseName = value;
                OnPropertyChanged();
            }
        }

        public string CourseDescription
        {
            get => _courseDescription;
            set
            {
                _courseDescription = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddDisciplineCommand { get; }
        public ICommand AddCourseCommand { get; }

        public AdminTeacherViewModel()
        {
            LoadData();

            AddDisciplineCommand = new RelayCommand(OnAddDiscipline);
            AddCourseCommand = new RelayCommand(OnAddCourse);
        }

        private void LoadData()
        {
            Disciplines = new ObservableCollection<Discipline>(_context.Disciplines.ToList());
            Courses = new ObservableCollection<Course>(_context.Courses.Include(c => c.Discipline).ToList());
        }

        private void OnAddDiscipline(object obj)
        {
            if (string.IsNullOrWhiteSpace(DisciplineName))
                return;

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
            if (SelectedDiscipline == null || string.IsNullOrWhiteSpace(CourseName))
                return;

            var course = new Course
            {
                Name = CourseName.Trim(),
                Description = string.IsNullOrWhiteSpace(CourseDescription) ? null : CourseDescription.Trim(),
                DisciplineId = SelectedDiscipline.Id,
                TeacherId = 1 // Предположим, текущий преподаватель — это ID=1
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            Courses.Add(course);

            CourseName = "";
            CourseDescription = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}