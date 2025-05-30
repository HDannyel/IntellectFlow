using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _context = new();
        private readonly int _studentId;

        public ObservableCollection<Course> EnrolledCourses { get; set; }

        public StudentViewModel(int studentId)
        {
            _studentId = studentId;
            LoadData();
        }

        private void LoadData()
        {
            EnrolledCourses = new ObservableCollection<Course>(
                _context.StudentCourses
                    .Where(sc => sc.StudentId == _studentId)
                    .Select(sc => sc.Course)
                    .Include(c => c.Discipline)
                    .ToList()
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}