using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        set { _newAssignmentTitle = value; OnPropertyChanged(nameof(NewAssignmentTitle)); }
    }

    private string _newAssignmentDescription = "";
    public string NewAssignmentDescription
    {
        get => _newAssignmentDescription;
        set { _newAssignmentDescription = value; OnPropertyChanged(nameof(NewAssignmentDescription)); }
    }

    private DateTime _newAssignmentDueDate = DateTime.Now.AddDays(7);
    public DateTime NewAssignmentDueDate
    {
        get => _newAssignmentDueDate;
        set { _newAssignmentDueDate = value; OnPropertyChanged(nameof(NewAssignmentDueDate)); }
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
        Course = _db.Courses
            .Where(c => c.Id == _courseId)
            .FirstOrDefault();

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
        return !string.IsNullOrWhiteSpace(NewAssignmentTitle);
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

        // Если файл загружен — создаем документ и связываем с заданием
        if (!string.IsNullOrEmpty(UploadedFilePath))
        {
            var doc = new Document
            {
                FileName = System.IO.Path.GetFileName(UploadedFilePath),
                FilePath = UploadedFilePath,
                ContentType = "application/octet-stream" // или более точный mime
            };

            _db.Documents.Add(doc);
            _db.SaveChanges();

            // TODO: Добавить связь Document <-> Assignment, если есть такая связь
            // Можно добавить ICollection<Document> в Assignment, если надо.
        }

        // Обновляем список
        Assignments.Add(assignment);

        // Очистка формы
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
