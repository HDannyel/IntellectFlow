using System;
using System.ComponentModel;

namespace IntellectFlow.DataModel
{
    public class StudentAssignment : INotifyPropertyChanged
    {
        private int _assignmentId;
        private string _title = "";
        private string _description = "";
        private DateTime _deadline = DateTime.Now;
        private bool _isSubmitted;
        private DateTime? _submissionDate;
        private string? _teacherComment;
        private string? _aiComment;
        private double? _grade;

        public int AssignmentId
        {
            get => _assignmentId;
            set { _assignmentId = value; OnPropertyChanged(nameof(AssignmentId)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public DateTime Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(nameof(Deadline)); }
        }

        public bool IsSubmitted
        {
            get => _isSubmitted;
            set { _isSubmitted = value; OnPropertyChanged(nameof(IsSubmitted)); }
        }

        public DateTime? SubmissionDate
        {
            get => _submissionDate;
            set { _submissionDate = value; OnPropertyChanged(nameof(SubmissionDate)); }
        }

        public string? TeacherComment
        {
            get => _teacherComment;
            set { _teacherComment = value; OnPropertyChanged(nameof(TeacherComment)); }
        }

        public string? AiComment
        {
            get => _aiComment;
            set { _aiComment = value; OnPropertyChanged(nameof(AiComment)); }
        }

        public double? Grade
        {
            get => _grade;
            set { _grade = value; OnPropertyChanged(nameof(Grade)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}