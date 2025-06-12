using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IntellectFlow.ViewModels
{
    public class StudentAssignmentViewModel : INotifyPropertyChanged
    {
        private Assignment _assignment;
        private StudentTaskSubmission _submission;
        private string _aiComment;
        private int? _aiScore;

        public Assignment Assignment
        {
            get => _assignment;
            set
            {
                _assignment = value;
                OnPropertyChanged(nameof(Assignment));
                OnPropertyChanged(nameof(AssignmentFilePath));
                OnPropertyChanged(nameof(HasAssignmentContent));
            }
        }

        private bool _isCheckingWithAI;
        public bool IsCheckingWithAI
        {
            get => _isCheckingWithAI;
            set
            {
                _isCheckingWithAI = value;
                OnPropertyChanged(nameof(IsCheckingWithAI));
                (CheckWithAICommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public StudentTaskSubmission Submission
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
                OnPropertyChanged(nameof(HasSubmissionContent));
            }
        }

        public string AiComment
        {
            get => Submission?.AiComment;
            set
            {
                if (Submission != null && Submission.AiComment != value)
                {
                    Submission.AiComment = value;
                    OnPropertyChanged(nameof(AiComment));
                }
            }
        }



        public int? AiScore
        {
            get => Submission?.AiScore;
            set
            {
                if (Submission != null && Submission.AiScore != value)
                {
                    Submission.AiScore = value;
                    OnPropertyChanged(nameof(AiScore));
                }
            }
        }


        public bool HasAssignmentContent =>
            (Assignment?.Document != null && !string.IsNullOrEmpty(Assignment.Document.FilePath) ||
            !string.IsNullOrEmpty(Assignment?.Description));

        public bool HasSubmissionContent =>
            (!string.IsNullOrEmpty(Submission?.SubmissionText) ||
            (Submission?.Document != null && !string.IsNullOrEmpty(Submission.Document.FilePath) &&
             File.Exists(Submission.Document.FilePath)));

        public string AssignmentFilePath => Assignment?.Document?.FilePath;

        public ICommand CheckWithAICommand { get; }

        public StudentAssignmentViewModel()
        {
            CheckWithAICommand = new AsyncRelayCommand(CheckWithAI,
                _ => HasSubmissionContent && HasAssignmentContent);
        }
        private async Task<string> ReadFileContentWithLimit(string filePath, int maxLength)
        {
            using var reader = new StreamReader(filePath);
            char[] buffer = new char[maxLength];
            int readChars = await reader.ReadBlockAsync(buffer, 0, maxLength);
            return new string(buffer, 0, readChars);
        }

        private async Task CheckWithAI(object _)
        {
            if (Submission == null || Assignment == null)
            {
                ShowErrorMessage("Не выбрано задание или решение");
                return;
            }

            try
            {
                string assignmentContent = "";
                string submissionContent = "";

                if (Assignment.Document != null && !string.IsNullOrEmpty(Assignment.Document.FilePath) && File.Exists(Assignment.Document.FilePath))
                {
                    assignmentContent = await ReadFileContentWithLimit(Assignment.Document.FilePath, 5000);
                }
                else if (!string.IsNullOrEmpty(Assignment.Description))
                {
                    assignmentContent = Assignment.Description;
                }
                else
                {
                    ShowErrorMessage("Нет задания для проверки");
                    return;
                }

                if (Submission.Document != null && !string.IsNullOrEmpty(Submission.Document.FilePath) && File.Exists(Submission.Document.FilePath))
                {
                    submissionContent = await ReadFileContentWithLimit(Submission.Document.FilePath, 5000);
                }
                else if (!string.IsNullOrEmpty(Submission.SubmissionText))
                {
                    submissionContent = Submission.SubmissionText;
                }
                else
                {
                    ShowErrorMessage("Нет решения для проверки");
                    return;
                }

                var service = new GigaChatService();
                var token = await service.GetAccessToken();

                IsCheckingWithAI = true;
                var (comment, score) = await service.AnalyzeSubmission(
      token,
      assignmentContent,
      submissionContent
  );

                AiComment = comment;
                AiScore = score;
                Submission.IsAiChecked = true;
                Submission.Status = SubmissionStatus.Checked;


                await SaveResultsToDatabase();
                ShowSuccessMessage($"Проверка завершена. Оценка: {score}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка анализа: {ex.Message}");
            }
            finally
            {
                IsCheckingWithAI = false;
            }
        }


        private void ShowErrorMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void ShowSuccessMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private async Task SaveResultsToDatabase()
        {
            try
            {
                await using var db = new IntellectFlowDbContext();
                var entity = await db.StudentTaskSubmissions.FindAsync(Submission.Id);
                if (entity != null)
                {
                    entity.AiComment = AiComment;
                    entity.AiScore = AiScore;
                    entity.IsAiChecked = true;
                    entity.Status = SubmissionStatus.Checked;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка сохранения: {ex.Message}");
            }
        }

        public string DisplayStatus => Submission?.Status switch
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
                    UpdateStatus();
                    OnPropertyChanged(nameof(TeacherScore));
                }
            }
        }

        public string TeacherComment
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

        public string UserAnswerText =>
            !string.IsNullOrEmpty(Submission?.SubmissionText)
                ? Submission.SubmissionText
                : "Ответ не предоставлен";

        public string SubmissionFilePath => Submission?.Document?.FilePath;

        private void UpdateStatus()
        {
            if (Submission == null) return;

            Submission.Status = TeacherScore switch
            {
                >= 3 and <= 5 => SubmissionStatus.Checked,
                1 or 2 => SubmissionStatus.Rejected,
                _ => Submission.Status
            };

            if (Submission.Status == SubmissionStatus.Rejected)
            {
                Submission.TeacherComment ??= "Требуется доработка";
            }

            OnPropertyChanged(nameof(DisplayStatus));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}