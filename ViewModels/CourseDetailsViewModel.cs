using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;
using IntellectFlow.Helpers;
using System.IO;
using Microsoft.EntityFrameworkCore;

public class CourseDetailsViewModel : INotifyPropertyChanged
{
    private readonly IntellectFlowDbContext _db;
    private readonly int _courseId;
    private readonly FileHelper _fileHelper;

    public CourseDetailsViewModel(IntellectFlowDbContext db, int courseId)
    {
        _db = db;
        _courseId = courseId;
        string rootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
        _fileHelper = new FileHelper(rootFolder);
        LoadCourseDetails();

        // Команды для заданий и лекций
        AddAssignmentCommand = new RelayCommand(_ => AddAssignment(), _ => CanAddAssignment());
        UploadFileCommand = new RelayCommand(_ => UploadFile());
        OpenAssignmentFileCommand = new RelayCommand(param => OpenAssignmentFile(param as Assignment), param => param is Assignment);
        OpenLectureFileCommand = new RelayCommand(param => OpenLectureFile(param as Lecture), param => param is Lecture);
        AddLectureCommand = new RelayCommand(_ => AddLecture(), _ => CanAddLecture());
        UploadLectureFileCommand = new RelayCommand(_ => UploadLectureFile());
        DeleteLectureCommand = new RelayCommand(param => DeleteLecture(param as Lecture), param => param is Lecture);
        DeleteAssignmentCommand = new RelayCommand(param => DeleteAssignment(param as Assignment), param => param is Assignment);

        // Команды для работы со студентами
        AddStudentToCourseCommand = new RelayCommand(_ => AddStudentToCourse(),
            _ => SelectedStudentToAdd != null && Course != null);
        RemoveStudentFromCourseCommand = new RelayCommand(_ => RemoveStudentFromCourse(),
            _ => SelectedStudentInCourse != null && Course != null);
    }

    // Основные свойства
    public Course? Course { get; private set; }
    public ObservableCollection<Assignment> Assignments { get; } = new ObservableCollection<Assignment>();
    public ObservableCollection<Lecture> Lectures { get; } = new ObservableCollection<Lecture>();

    // Свойства для работы со студентами
    public ObservableCollection<Student> StudentsInCourse { get; } = new ObservableCollection<Student>();
    public ObservableCollection<Student> AvailableStudents { get; } = new ObservableCollection<Student>();

    private Student? _selectedStudentInCourse;
    public Student? SelectedStudentInCourse
    {
        get => _selectedStudentInCourse;
        set
        {
            if (_selectedStudentInCourse != value)
            {
                _selectedStudentInCourse = value;
                OnPropertyChanged(nameof(SelectedStudentInCourse));
                (RemoveStudentFromCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    private Student? _selectedStudentToAdd;
    public Student? SelectedStudentToAdd
    {
        get => _selectedStudentToAdd;
        set
        {
            if (_selectedStudentToAdd != value)
            {
                _selectedStudentToAdd = value;
                OnPropertyChanged(nameof(SelectedStudentToAdd));
                (AddStudentToCourseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    // Свойства для заданий
    private string _newAssignmentTitle = "";
    public string NewAssignmentTitle
    {
        get => _newAssignmentTitle;
        set
        {
            if (_newAssignmentTitle != value)
            {
                _newAssignmentTitle = value;
                OnPropertyChanged(nameof(NewAssignmentTitle));
                RaiseCanExecuteChanged();
            }
        }
    }

    private string _newAssignmentDescription = "";
    public string NewAssignmentDescription
    {
        get => _newAssignmentDescription;
        set
        {
            if (_newAssignmentDescription != value)
            {
                _newAssignmentDescription = value;
                OnPropertyChanged(nameof(NewAssignmentDescription));
                RaiseCanExecuteChanged();
            }
        }
    }

    private DateTime _newAssignmentDueDate = DateTime.Now.AddDays(7);
    public DateTime NewAssignmentDueDate
    {
        get => _newAssignmentDueDate;
        set
        {
            if (_newAssignmentDueDate != value)
            {
                _newAssignmentDueDate = value;
                OnPropertyChanged(nameof(NewAssignmentDueDate));
                RaiseCanExecuteChanged();
            }
        }
    }

    private string? _uploadedFilePath;
    public string? UploadedFilePath
    {
        get => _uploadedFilePath;
        set { _uploadedFilePath = value; OnPropertyChanged(nameof(UploadedFilePath)); }
    }

    // Свойства для лекций
    private string _newLectureTitle = "";
    public string NewLectureTitle
    {
        get => _newLectureTitle;
        set
        {
            if (_newLectureTitle != value)
            {
                _newLectureTitle = value;
                OnPropertyChanged(nameof(NewLectureTitle));
                RaiseCanExecuteChanged();
            }
        }
    }

    private string? _uploadedLectureFilePath;
    public string? UploadedLectureFilePath
    {
        get => _uploadedLectureFilePath;
        set { _uploadedLectureFilePath = value; OnPropertyChanged(nameof(UploadedLectureFilePath)); }
    }

    // Команды
    public ICommand AddAssignmentCommand { get; }
    public ICommand UploadFileCommand { get; }
    public ICommand OpenAssignmentFileCommand { get; }
    public ICommand OpenLectureFileCommand { get; }
    public ICommand AddLectureCommand { get; }
    public ICommand UploadLectureFileCommand { get; }
    public ICommand DeleteLectureCommand { get; }
    public ICommand DeleteAssignmentCommand { get; }
    public ICommand AddStudentToCourseCommand { get; }
    public ICommand RemoveStudentFromCourseCommand { get; }

    private void LoadCourseDetails()
    {
        Course = _db.Courses
            .Include(c => c.Discipline)
                .ThenInclude(d => d.Teacher)
            .FirstOrDefault(c => c.Id == _courseId);

        if (Course == null) return;

        // Загрузка заданий
        Assignments.Clear();
        foreach (var a in _db.Assignments.Where(a => a.CourseId == _courseId))
            Assignments.Add(a);

        // Загрузка лекций
        Lectures.Clear();
        foreach (var l in _db.Lectures.Where(l => l.CourseId == _courseId))
            Lectures.Add(l);

        // Загрузка студентов
        LoadStudentsInCourse();
        UpdateAvailableStudents();
    }

    #region Методы для работы со студентами

    private void LoadStudentsInCourse()
    {
        StudentsInCourse.Clear();
        if (Course == null) return;

        var students = _db.StudentCourses
            .Where(sc => sc.CourseId == Course.Id)
            .Select(sc => sc.Student)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.Name)
            .ToList();

        foreach (var student in students)
            StudentsInCourse.Add(student);
    }

    private void UpdateAvailableStudents()
    {
        AvailableStudents.Clear();
        if (Course == null) return;

        var studentsNotInCourse = _db.Students
            .Where(s => !s.StudentCourses.Any(sc => sc.CourseId == Course.Id))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.Name)
            .ToList();

        foreach (var student in studentsNotInCourse)
            AvailableStudents.Add(student);
    }

    private void AddStudentToCourse()
    {
        if (Course == null || SelectedStudentToAdd == null) return;

        // Проверяем, не добавлен ли уже студент
        var exists = _db.StudentCourses
            .Any(sc => sc.CourseId == Course.Id && sc.StudentId == SelectedStudentToAdd.Id);

        if (exists) return;

        var studentCourse = new StudentCourse
        {
            CourseId = Course.Id,
            StudentId = SelectedStudentToAdd.Id,
            EnrolledDate = DateTime.UtcNow
        };

        _db.StudentCourses.Add(studentCourse);
        _db.SaveChanges();

        // Обновляем списки
        LoadStudentsInCourse();
        UpdateAvailableStudents();

        SelectedStudentToAdd = null;
    }

    private void RemoveStudentFromCourse()
    {
        if (Course == null || SelectedStudentInCourse == null) return;

        var studentCourse = _db.StudentCourses
            .FirstOrDefault(sc => sc.CourseId == Course.Id && sc.StudentId == SelectedStudentInCourse.Id);

        if (studentCourse != null)
        {
            _db.StudentCourses.Remove(studentCourse);
            _db.SaveChanges();

            // Обновляем списки
            LoadStudentsInCourse();
            UpdateAvailableStudents();

            SelectedStudentInCourse = null;
        }
    }

    #endregion

    #region Методы для заданий

    private bool CanAddAssignment()
    {
        return !string.IsNullOrWhiteSpace(NewAssignmentTitle)
               && !string.IsNullOrWhiteSpace(NewAssignmentDescription)
               && NewAssignmentDueDate != default;
    }

    private void AddAssignment()
    {
        if (Course == null) return;

        var teacherName = Course.Teacher.FullName ?? "DefaultTeacher";
        var disciplineName = Course.Discipline?.Name ?? "DefaultDiscipline";
        var courseName = Course.Name ?? "DefaultCourse";

        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Teachers", teacherName, disciplineName, courseName, "Assignments");

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
            Debug.WriteLine("Created directory: " + basePath);
        }

        Document? doc = null;

        if (!string.IsNullOrEmpty(UploadedFilePath))
        {
            string destFileName = Path.Combine(basePath, Path.GetFileName(UploadedFilePath));
            File.Copy(UploadedFilePath, destFileName, overwrite: true);

            doc = new Document
            {
                FileName = Path.GetFileName(destFileName),
                FilePath = destFileName,
                ContentType = "application/octet-stream"
            };
            _db.Documents.Add(doc);
            _db.SaveChanges();
        }

        var assignment = new Assignment
        {
            Title = NewAssignmentTitle,
            Description = NewAssignmentDescription,
            DueDate = NewAssignmentDueDate,
            CourseId = Course.Id,
            DocumentId = doc?.Id
        };

        _db.Assignments.Add(assignment);
        _db.SaveChanges();

        Assignments.Add(assignment);

        NewAssignmentTitle = "";
        NewAssignmentDescription = "";
        UploadedFilePath = null;
    }

    private void UploadFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
            UploadedFilePath = openFileDialog.FileName;
    }

    private void OpenAssignmentFile(Assignment? assignment)
    {
        if (assignment == null) return;

        var doc = _db.Documents.FirstOrDefault(d => d.Id == assignment.DocumentId);
        if (doc == null || string.IsNullOrEmpty(doc.FilePath)) return;

        try
        {
            Process.Start(new ProcessStartInfo(doc.FilePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
        }
    }

    private void DeleteAssignment(Assignment? assignment)
    {
        if (assignment == null) return;

        var assignmentInDb = _db.Assignments.Find(assignment.Id);
        if (assignmentInDb != null)
        {
            var doc = _db.Documents.Find(assignmentInDb.DocumentId);
            if (doc != null)
            {
                _db.Documents.Remove(doc);
            }

            _db.Assignments.Remove(assignmentInDb);
            _db.SaveChanges();

            Assignments.Remove(assignment);
        }
    }

    #endregion

    #region Методы для лекций

    private bool CanAddLecture()
    {
        return !string.IsNullOrWhiteSpace(NewLectureTitle)
               && !string.IsNullOrEmpty(UploadedLectureFilePath);
    }

    private void AddLecture()
    {
        if (Course == null) return;

        var teacherName = Course.Teacher.FullName ?? "DefaultTeacher";
        var disciplineName = Course.Discipline?.Name ?? "DefaultDiscipline";
        var courseName = Course.Name ?? "DefaultCourse";

        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Teachers", teacherName, disciplineName, courseName, "Lectures");

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
            Debug.WriteLine("Created directory: " + basePath);
        }

        if (string.IsNullOrEmpty(UploadedLectureFilePath))
            return;

        string destFileName = Path.Combine(basePath, Path.GetFileName(UploadedLectureFilePath));
        File.Copy(UploadedLectureFilePath, destFileName, overwrite: true);

        var doc = new Document
        {
            FileName = Path.GetFileName(destFileName),
            FilePath = destFileName,
            ContentType = "application/octet-stream"
        };

        _db.Documents.Add(doc);
        _db.SaveChanges();

        var lecture = new Lecture
        {
            Title = NewLectureTitle,
            DocumentId = doc.Id,
            CourseId = Course.Id
        };
        _db.Lectures.Add(lecture);
        _db.SaveChanges();

        Lectures.Add(lecture);

        NewLectureTitle = "";
        UploadedLectureFilePath = null;
    }

    private void OpenLectureFile(Lecture? lecture)
    {
        if (lecture == null) return;

        var doc = _db.Documents.FirstOrDefault(d => d.Id == lecture.DocumentId);
        if (doc == null || string.IsNullOrEmpty(doc.FilePath)) return;

        try
        {
            Process.Start(new ProcessStartInfo(doc.FilePath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
        }
    }

    private void UploadLectureFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
            UploadedLectureFilePath = openFileDialog.FileName;
    }

    private void DeleteLecture(Lecture? lecture)
    {
        if (lecture == null) return;

        var lectureInDb = _db.Lectures.Find(lecture.Id);
        if (lectureInDb != null)
        {
            var doc = _db.Documents.Find(lectureInDb.DocumentId);
            if (doc != null)
            {
                _db.Documents.Remove(doc);
            }

            _db.Lectures.Remove(lectureInDb);
            _db.SaveChanges();

            Lectures.Remove(lecture);
        }
    }

    #endregion

    private void RaiseCanExecuteChanged()
    {
        if (AddAssignmentCommand is RelayCommand relayAssign)
            relayAssign.RaiseCanExecuteChanged();

        if (AddLectureCommand is RelayCommand relayLecture)
            relayLecture.RaiseCanExecuteChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}