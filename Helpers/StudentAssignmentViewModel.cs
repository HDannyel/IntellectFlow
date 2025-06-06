using IntellectFlow.DataModel;
using System.ComponentModel;

namespace IntellectFlow.Helpers
{
    public class StudentAssignmentViewModel : INotifyPropertyChanged
    {
        public Assignment Assignment { get; set; } = null!;
        private StudentTaskSubmission? _submission;

        public StudentTaskSubmission? Submission
        {
            get => _submission;
            set
            {
                _submission = value;
                OnPropertyChanged(nameof(Submission));
                OnPropertyChanged(nameof(DisplayStatus));
                OnPropertyChanged(nameof(TeacherComment));
                OnPropertyChanged(nameof(TeacherScore));
                OnPropertyChanged(nameof(AiComment));
                OnPropertyChanged(nameof(AiScore));
                OnPropertyChanged(nameof(UserAnswerText));
                OnPropertyChanged(nameof(SubmissionFilePath));
            }
        }

        public string DisplayStatus =>
            Submission?.Status switch
            {
                SubmissionStatus.Pending => "Ожидает сдачи",
                SubmissionStatus.Submitted => "Отправлено",
                SubmissionStatus.Checked => "Проверено",
                SubmissionStatus.Rejected => "Отклонено",
                _ => "Неизвестно"
            };

        public int? TeacherScore
        {
            get => Submission?.TeacherScore;
            set
            {
                if (Submission != null && Submission.TeacherScore != value)
                {
                    Submission.TeacherScore = value;
                    UpdateStatusByScore();
                    OnPropertyChanged(nameof(TeacherScore));
                }
            }
        }

        public string? TeacherComment
        {
            get => Submission?.TeacherComment;
            set
            {
                if (Submission != null && Submission.TeacherComment != value)
                {
                    Submission.TeacherComment = value;
                    OnPropertyChanged(nameof(TeacherComment));
                }
            }
        }

        public string? AiComment => Submission?.AiComment;
        public int? AiScore => Submission?.AiScore;

        public string UserAnswerText => Submission?.SubmissionText
                                   ?? "Ответ не предоставлен";

        public string? SubmissionFilePath => Submission?.Document?.FilePath;

        private void UpdateStatusByScore()
        {
            if (Submission == null) return;

            if (Submission.TeacherScore >= 3 && Submission.TeacherScore <= 5)
            {
                Submission.Status = SubmissionStatus.Checked;
            }
            else if (Submission.TeacherScore == 1 || Submission.TeacherScore == 2)
            {
                Submission.TeacherComment ??= "Задание требует доработки.";
                Submission.Status = SubmissionStatus.Rejected;
            }

            OnPropertyChanged(nameof(DisplayStatus));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}