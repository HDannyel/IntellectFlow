using IntellectFlow.DataModel;
using IntellectFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IntellectFlow.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
        }
        private void DisciplinesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem is Discipline selectedDiscipline)
            {
                // Например: показать информацию о дисциплине
                MessageBox.Show($"Вы выбрали дисциплину: {selectedDiscipline.Name}");
            }
        }
        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            var addStudentWindow = new AddStudentWindow();
            if (addStudentWindow.ShowDialog() == true)
            {
                var newStudent = addStudentWindow.NewStudent;
                if (newStudent != null)
                {
                    // Добавить нового студента в коллекцию, например, через DataContext
                    if (DataContext is AdminViewModel vm)
                    {
                        vm.Students.Add(newStudent);
                    }
                }
            }
        }

        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            var addTeacherWindow = new AddTeacherWindow();
            if (addTeacherWindow.ShowDialog() == true)
            {
                var newTeacher = addTeacherWindow.NewTeacher;
                if (newTeacher != null)
                {
                    if (DataContext is AdminViewModel vm)
                    {
                        vm.Teachers.Add(newTeacher);
                    }
                }
            }
        }

    }
}
