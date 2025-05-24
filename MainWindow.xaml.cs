using IntellectFlow.Models;
using IntellectFlow.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntellectFlow // Или IntellectFlow.Views, если в XAML `x:Class="IntellectFlow.MainWindow"`
{
    public partial class MainWindow : Window
    {
        private readonly IntellectFlowDbContext _context;
        private readonly UserService _userService;

        public MainWindow()
        {
            InitializeComponent(); // ← это ссылается на MainWindow.g.cs, генерируемый из XAML
            _context = new IntellectFlowDbContext();
            _userService = new UserService(_context);
        }

        private void AddTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            var addTeacherWindow = new AddTeacherWindow();
            bool? result = addTeacherWindow.ShowDialog();

            if (result == true)
            {
                var teacher = addTeacherWindow.NewTeacher;
                var (login, password) = _userService.CreateTeacher(teacher.Name, teacher.MidleName, teacher.LastName);

                MessageBox.Show($"Преподаватель создан!\nЛогин: {login}\nПароль: {password}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
