using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Diagnostics;

public class CourseDetailsViewModel : INotifyPropertyChanged
{
    private readonly IntellectFlowDbContext _db;
    private readonly int _courseId;

    public CourseDetailsViewModel(IntellectFlowDbContext db, int courseId)
    {
        _db = db;
        _courseId = courseId;

        LoadCourseDetails();

        AddAssignmentCommand = new RelayCommand(_ => AddAssignment(), _ => CanAddAssignment());
        UploadFileCommand = new RelayCommand(_ => UploadFile());
        OpenAssignmentFileCommand = new RelayCommand(param => OpenAssignmentFile(param as Assignment), param => param is Assignment);
        OpenLectureFileCommand = new RelayCommand(param => OpenLectureFile(param as Lecture), param => param is Lecture);
        AddLectureCommand = new RelayCommand(_ => AddLecture(), _ => CanAddLecture());
        UploadLectureFileCommand = new RelayCommand(_ => UploadLectureFile());

        DeleteLectureCommand = new RelayCommand(param => DeleteLecture(param as Lecture), param => param is Lecture);
        DeleteAssignmentCommand = new RelayCommand(param => DeleteAssignment(param as Assignment), param => param is Assignment);
    }

    public Course? Course { get; private set; }
    public ObservableCollection<Assignment> Assignments { get; } = new ObservableCollection<Assignment>();
    public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();
    public ObservableCollection<Lecture> Lectures { get; } = new ObservableCollection<Lecture>();

    // --- Для заданий ---
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

    // --- Для лекций ---
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

    private void LoadCourseDetails()
    {
        Course = _db.Courses.FirstOrDefault(c => c.Id == _courseId);
        if (Course == null) return;

        Assignments.Clear();
        foreach (var a in _db.Assignments.Where(a => a.CourseId == _courseId))
            Assignments.Add(a);

        Students.Clear();
        var students = _db.StudentCourses
            .Where(sc => sc.CourseId == _courseId)
            .Select(sc => sc.Student)
            .ToList();
        foreach (var s in students)
            Students.Add(s);

        Lectures.Clear();
        var lectures = _db.Lectures.Where(l => l.CourseId == _courseId).ToList();
        foreach (var lecture in lectures)
            Lectures.Add(lecture);
    }

    // Проверка для добавления задания
    private bool CanAddAssignment()
    {
        return !string.IsNullOrWhiteSpace(NewAssignmentTitle)
               && !string.IsNullOrWhiteSpace(NewAssignmentDescription)
               && NewAssignmentDueDate != default;
    }

    private void AddAssignment()
    {
        if (Course == null) return;

        Document? doc = null;

        if (!string.IsNullOrEmpty(UploadedFilePath))
        {
            doc = new Document
            {
                FileName = System.IO.Path.GetFileName(UploadedFilePath),
                FilePath = UploadedFilePath,
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
            DocumentId = doc?.Id  // Связываем с документом, если он есть
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

    // Проверка для добавления лекции
    private bool CanAddLecture()
    {
        return !string.IsNullOrWhiteSpace(NewLectureTitle)
               && !string.IsNullOrEmpty(UploadedLectureFilePath);
    }

    private void AddLecture()
    {
        if (Course == null) return;

        var doc = new Document
        {
            FileName = System.IO.Path.GetFileName(UploadedLectureFilePath!),
            FilePath = UploadedLectureFilePath!,
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
