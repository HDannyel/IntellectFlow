using IntellectFlow.DataModel;
using IntellectFlow.Models;
using IntellectFlow.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace IntellectFlow.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _db;
        private readonly int _studentId;

        public StudentViewModel(IntellectFlowDbContext db, int studentId)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _studentId = studentId;

            OpenCourseDetailsCommand = new RelayCommand(_ => OpenCourseDetails(), _ => SelectedCourse != null);

            LoadData();
        }

        public ObservableCollection<Discipline> MyDisciplines { get; } = new ObservableCollection<Discipline>();
        public ObservableCollection<Course> MyCourses { get; } = new ObservableCollection<Course>();

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
                    ((RelayCommand)OpenCourseDetailsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand OpenCourseDetailsCommand { get; }

        private void LoadData()
        {
            var courses = _db.StudentCourses
                .Where(sc => sc.StudentId == _studentId)
                .Include(sc => sc.Course) // сначала загружаем Course
                    .ThenInclude(c => c.Discipline) // и Discipline у Course
                .Select(sc => sc.Course)
                .Distinct()
                .OrderBy(c => c.Name)
                .ToList();

            MyCourses.Clear();
            foreach (var course in courses)
                MyCourses.Add(course);

            var disciplines = courses
                .Select(c => c.Discipline)
                .Where(d => d != null)
                .Distinct()
                .OrderBy(d => d.Name)
                .ToList();

            MyDisciplines.Clear();
            foreach (var discipline in disciplines)
                MyDisciplines.Add(discipline!);
        }

        private void OpenCourseDetails()
        {
            if (SelectedCourse == null) return;

            var courseDetailsVM = new CourseDetailsViewModel(_db, SelectedCourse.Id);
            var courseDetailsWindow = new CourseDetailsWindow
            {
                DataContext = courseDetailsVM
            };
            courseDetailsWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
