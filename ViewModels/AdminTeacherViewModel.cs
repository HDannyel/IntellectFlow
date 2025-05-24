using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
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

        // Коллекции и выбранные элементы
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public Discipline SelectedDiscipline { get; set; }

        private ObservableCollection<Course> _courses;
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set
            {
                _courses = value;
                OnPropertyChanged();
            }
        }

        public Course SelectedCourse { get; set; }

        private string _disciplineName = "";
        private string _disciplineDescription = "";

        private string _courseName = "";
        private string _courseDescription = "";

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

        public ICommand AddDisciplineCommand { get; }
        public ICommand AddCourseCommand { get; }



        private ObservableCollection<Course> _allCourses; // Все курсы без фильтрации

        public AdminTeacherViewModel()
        {
            LoadData();

            AddDisciplineCommand = new RelayCommand(OnAddDiscipline);
            AddCourseCommand = new RelayCommand(OnAddCourse);
        }
       

        private void LoadData()
        {
            Disciplines = new ObservableCollection<Discipline>(_context.Disciplines.ToList());

            _allCourses = new ObservableCollection<Course>(_context.Courses.Include(c => c.Discipline).ToList());
            Courses = new ObservableCollection<Course>(_allCourses);
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
                TeacherId = 1 // Пример: текущий преподаватель - ID=1
            };

            _context.Courses.Add(course);
            _context.SaveChanges();

            _allCourses.Add(course);
            Courses.Add(course);

            CourseName = "";
            CourseDescription = "";
        }

        // Метод фильтрации курсов по выбранной дисциплине
        public void FilterCoursesByDiscipline(Discipline discipline)
        {
            if (discipline == null)
            {
                Courses = new ObservableCollection<Course>(_allCourses);
            }
            else
            {
                var filtered = _allCourses.Where(c => c.DisciplineId == discipline.Id);
                Courses = new ObservableCollection<Course>(filtered);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
