using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

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
    }

    public Course? Course { get; private set; }
    public ObservableCollection<Assignment> Assignments { get; } = new ObservableCollection<Assignment>();
    public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();

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

    // Команды
    public ICommand AddAssignmentCommand { get; }
    public ICommand UploadFileCommand { get; }

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
    }

    private bool CanAddAssignment()
    {
        // Можно добавить проверку Description и DueDate, если нужно
        return !string.IsNullOrWhiteSpace(NewAssignmentTitle)
               && !string.IsNullOrWhiteSpace(NewAssignmentDescription)
               && NewAssignmentDueDate != default;
    }

    private void AddAssignment()
    {
        if (Course == null) return;

        var assignment = new Assignment
        {
            Title = NewAssignmentTitle,
            Description = NewAssignmentDescription,
            DueDate = NewAssignmentDueDate,
            CourseId = Course.Id
        };

        _db.Assignments.Add(assignment);
        _db.SaveChanges();

        if (!string.IsNullOrEmpty(UploadedFilePath))
        {
            var doc = new Document
            {
                FileName = System.IO.Path.GetFileName(UploadedFilePath),
                FilePath = UploadedFilePath,
                ContentType = "application/octet-stream" // или точный mime
            };

            _db.Documents.Add(doc);
            _db.SaveChanges();

            // TODO: связать Document с Assignment, если нужно
        }

        Assignments.Add(assignment);

        NewAssignmentTitle = "";
        NewAssignmentDescription = "";
        UploadedFilePath = null;
    }

    private void UploadFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            UploadedFilePath = openFileDialog.FileName;
        }
    }

    private void RaiseCanExecuteChanged()
    {
        if (AddAssignmentCommand is RelayCommand relayCommand)
            relayCommand.RaiseCanExecuteChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
