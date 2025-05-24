using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IntellectFlow.DataModel;
using IntellectFlow.Models;
using IntellectFlow.ViewModels;

namespace IntellectFlow.Views
{
    public partial class AdminTeacherView : UserControl
    {
        private readonly IntellectFlowDbContext _context = new IntellectFlowDbContext();
        private readonly UserService _userService;

        public AdminTeacherView()
        {
            InitializeComponent();
            _userService = new UserService(_context);
            DataContext = new AdminTeacherViewModel();

            // Можно подписаться на команды, если не используешь MVVM команду
            // или перенести в VM
        }
        private void DisciplinesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Здесь можно обработать двойной клик по дисциплине.
            // Например:
            if (((ListBox)sender).SelectedItem is Discipline selectedDiscipline)
            {
                MessageBox.Show($"Вы выбрали дисциплину: {selectedDiscipline.Name}");
            }
        }
        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTeacherWindow();
            if (window.ShowDialog() == true)
            {
                var teacher = window.NewTeacher;
                var (login, password) = _userService.CreateTeacher(teacher.Name, teacher.MidleName, teacher.LastName);

                MessageBox.Show($"Преподаватель создан!\nЛогин: {login}\nПароль: {password}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddStudentWindow();
            if (window.ShowDialog() == true)
            {
                var student = window.NewStudent;
                var (login, password) = _userService.CreateStudent(student.Name, student.MidleName, student.LastName);

                MessageBox.Show($"Студент создан!\nЛогин: {login}\nПароль: {password}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
