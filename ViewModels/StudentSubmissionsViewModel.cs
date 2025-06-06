using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace IntellectFlow.ViewModels
{
    public class StudentSubmissionsViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _db;
        private readonly int _studentId;
        private readonly int _courseId;

        public ObservableCollection<StudentAssignmentViewModel> StudentAssignments { get; } = new();

        private StudentAssignmentViewModel? _selectedAssignment;
        public StudentAssignmentViewModel? SelectedAssignment
        {
            get => _selectedAssignment;
            set
            {
                if (_selectedAssignment != value)
                {
                    _selectedAssignment = value;
                    OnPropertyChanged(nameof(SelectedAssignment));
                    (SaveChangesCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveChangesCommand { get; }
        public ICommand OpenSubmissionFileCommand { get; }

        public StudentSubmissionsViewModel(IntellectFlowDbContext db, int courseId, int studentId)
        {
            _db = db;
            _courseId = courseId;
            _studentId = studentId;

            LoadStudentAssignments();

            SaveChangesCommand = new RelayCommand(SaveChanges, _ => CanSaveChanges());
            OpenSubmissionFileCommand = new RelayCommand(OpenSubmissionFile, CanOpenSubmissionFile);
        }

        private bool CanSaveChanges()
        {
            return SelectedAssignment?.Submission != null;
        }

        private bool CanOpenSubmissionFile(object? param)
        {
            return SelectedAssignment?.Submission?.Document != null;
        }

        private void SaveChanges(object? param)
        {
            if (SelectedAssignment?.Submission == null) return;

            var submission = SelectedAssignment.Submission;

            try
            {
                if (submission.Id == 0)
                {
                    _db.StudentTaskSubmissions.Add(submission);
                }
                else
                {
                    var entry = _db.StudentTaskSubmissions.Update(submission);
                    entry.Property(s => s.AssignmentId).IsModified = false;
                    entry.Property(s => s.StudentId).IsModified = false;
                }

                _db.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadStudentAssignments(); // Обновляем данные
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenSubmissionFile(object? param)
        {
            if (SelectedAssignment?.Submission?.Document is Document doc && !string.IsNullOrEmpty(doc.FilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(doc.FilePath)
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadStudentAssignments()
        {
            var assignments = _db.Assignments.Where(a => a.CourseId == _courseId).ToList();
            var submissions = _db.StudentTaskSubmissions
                .Where(s => s.StudentId == _studentId)
                .ToList();

            StudentAssignments.Clear();
            foreach (var assignment in assignments)
            {
                var submission = submissions.FirstOrDefault(s => s.AssignmentId == assignment.Id);
                StudentAssignments.Add(new StudentAssignmentViewModel
                {
                    Assignment = assignment,
                    Submission = submission ?? new StudentTaskSubmission
                    {
                        Status = SubmissionStatus.Pending,
                        SubmissionText = ""
                    }
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}