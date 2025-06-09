using IntellectFlow.DataModel;
using IntellectFlow.Models;
using IntellectFlow.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Windows;

namespace IntellectFlow.ViewModels
{
    public class StudentCourseDetailsViewModel : INotifyPropertyChanged
    {
        private readonly IntellectFlowDbContext _db;
        private readonly int _courseId;
        private readonly int _studentId;
        private readonly FileHelper _fileHelper;
        private DispatcherTimer _timer;
        public Course? Course { get; private set; }
        public ObservableCollection<Lecture> Lectures { get; } = new();
        public ObservableCollection<Assignment> Assignments { get; } = new();
        public ObservableCollection<StudentAssignmentViewModel> StudentAssignments { get; } = new();

        public StudentCourseDetailsViewModel(IntellectFlowDbContext db, int courseId, int studentId)
        {
            _db = db;
            _courseId = courseId;
            _studentId = studentId;
            _fileHelper = new FileHelper(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files"));

            LoadCourseDetails();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _timer.Tick += (s, e) => LoadCourseDetails();
            _timer.Start();
        }

        private void LoadCourseDetails()
        {
            Course = _db.Courses
                .Include(c => c.Discipline)
                    .ThenInclude(d => d.Teacher)
                .FirstOrDefault(c => c.Id == _courseId);

            if (Course == null) return;

            Lectures.Clear();
            foreach (var lecture in _db.Lectures.Where(l => l.CourseId == _courseId))
                Lectures.Add(lecture);

            var assignments = _db.Assignments
                .Include(a => a.Document)
                .Where(a => a.CourseId == _courseId)
                .ToList();

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
                    Submission = submission
                });
            }
        }

        private string GetStudentName()
        {
            var student = _db.Students.FirstOrDefault(s => s.Id == _studentId);
            return student?.FullName ?? "DefaultStudent";
        }

        public ICommand DownloadLectureFileCommand => new RelayCommand(param => DownloadLectureFile(param as Lecture));
        public ICommand DownloadAssignmentCommand => new RelayCommand(param => DownloadAssignment(param as Assignment));
        public ICommand UploadSolutionCommand => new RelayCommand(param => UploadSolution(param as Assignment));
        public ICommand UploadAssignmentCommand => new RelayCommand(param => UploadAssignment(param as Assignment));


        private void UploadAssignment(Assignment assignment)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Получаем данные преподавателя из контекста
                string teacherName = Course?.Discipline?.Teacher?.FullName ?? "DefaultTeacher";
                string disciplineName = Course?.Discipline?.Name ?? "DefaultDiscipline";
                string courseName = Course?.Name ?? "DefaultCourse";

                // Генерируем путь через FileHelper
                string destFilePath = _fileHelper.GetTeacherFilePath(
                    teacherName,
                    disciplineName,
                    courseName,
                    "Assignments",
                    Path.GetFileName(filePath)
                );

                // Копируем файл
                File.Copy(filePath, destFilePath, overwrite: true);

                // Создаем или обновляем документ
                if (assignment.Document == null)
                {
                    assignment.Document = new Document
                    {
                        FileName = Path.GetFileName(destFilePath),
                        FilePath = destFilePath,
                        ContentType = MimeTypes.GetMimeType(destFilePath)
                    };
                    _db.Documents.Add(assignment.Document);
                }
                else
                {
                    assignment.Document.FileName = Path.GetFileName(destFilePath);
                    assignment.Document.FilePath = destFilePath;
                    assignment.Document.ContentType = MimeTypes.GetMimeType(destFilePath);
                }

                _db.SaveChanges();
                LoadCourseDetails();
                MessageBox.Show("Файл задания успешно загружен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DownloadAssignment(Assignment assignment)
        {
            if (assignment?.Document == null) return;

            // Пытаемся найти файл по сохраненному пути
            if (File.Exists(assignment.Document.FilePath))
            {
                DownloadFile(assignment.Document.FilePath);
                return;
            }

            // Если файл не найден, пробуем восстановить путь
            string expectedPath = _fileHelper.GetTeacherFilePath(
                Course?.Discipline?.Teacher?.FullName ?? "DefaultTeacher",
                Course?.Discipline?.Name ?? "DefaultDiscipline",
                Course?.Name ?? "DefaultCourse",
                "Assignments",
                assignment.Document.FileName
            );

            if (File.Exists(expectedPath))
            {
                // Обновляем путь в базе
                assignment.Document.FilePath = expectedPath;
                _db.SaveChanges();
                DownloadFile(expectedPath);
            }
            else
            {
                MessageBox.Show("Файл задания не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadLectureFile(Lecture? lecture)
        {
            if (lecture == null || lecture.DocumentId == null) return;

            var doc = _db.Documents.FirstOrDefault(d => d.Id == lecture.DocumentId);
            if (doc != null && File.Exists(doc.FilePath))
            {
                DownloadFile(doc.FilePath);
            }
        }

        private void DownloadFile(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
                return;

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = Path.GetFileName(sourceFilePath),
                Filter = "Все файлы|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.Copy(sourceFilePath, dlg.FileName, overwrite: true);
                    Debug.WriteLine("Файл успешно сохранён: " + dlg.FileName);
                    // Здесь можно добавить всплывающее уведомление пользователю
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка при сохранении файла: " + ex.Message);
                    // Можно показать сообщение об ошибке пользователю
                }
            }
        }
        private StudentAssignmentViewModel? _selectedStudentAssignment;
        public StudentAssignmentViewModel? SelectedStudentAssignment
        {
            get => _selectedStudentAssignment;
            set
            {
                _selectedStudentAssignment = value;
                OnPropertyChanged(nameof(SelectedStudentAssignment));
            }
        }
        private void UploadSolution(Assignment? assignment)
        {
            if (assignment == null) return;

            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string solutionPath = openFileDialog.FileName;

                string studentName = GetStudentName();
                string disciplineName = Course?.Discipline?.Name ?? "DefaultDiscipline";
                string courseName = Course?.Name ?? "DefaultCourse";

                string destFilePath = _fileHelper.GetStudentFilePath(
                    studentName,
                    disciplineName,
                    courseName,
                    "StudentTask",
                    Path.GetFileName(solutionPath)
                );

                File.Copy(solutionPath, destFilePath, overwrite: true);

                var doc = new Document
                {
                    FileName = Path.GetFileName(destFilePath),
                    FilePath = destFilePath,
                    ContentType = "application/octet-stream"
                };

                _db.Documents.Add(doc);
                _db.SaveChanges();

                string userAnswerText = $"Решение загружено из файла {doc.FileName}";

                var newSubmission = new StudentTaskSubmission
                {
                    StudentId = _studentId,
                    AssignmentId = assignment.Id,
                    SubmissionText = userAnswerText,
                    SubmissionDate = DateTime.UtcNow,
                    Status = SubmissionStatus.Submitted,
                    DocumentId = doc.Id
                };

                _db.StudentTaskSubmissions.Add(newSubmission);
                _db.SaveChanges();

                LoadCourseDetails();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
