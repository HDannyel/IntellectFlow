using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;

        public AdminView(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
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
        private async void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            var addStudentWindow = new AddStudentWindow();
            if (addStudentWindow.ShowDialog() == true)
            {
                var newStudent = addStudentWindow.NewStudent;
                if (newStudent != null)
                {
                    var userService = _serviceProvider.GetRequiredService<UserService>();

                    try
                    {
                        var (login, password) = await userService.CreateStudentAsync(
                            newStudent.Name,
                            newStudent.MiddleName,
                            newStudent.LastName);

                        MessageBox.Show($"Студент добавлен!\nЛогин: {login}\nПароль: {password}");

                        if (DataContext is AdminViewModel vm)
                        {
                            vm.Students.Add(newStudent);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении студента: {ex.Message}");
                    }
                }
            }
        }


        private async void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            var addTeacherWindow = new AddTeacherWindow();
            if (addTeacherWindow.ShowDialog() == true)
            {
                var newTeacher = addTeacherWindow.NewTeacher;
                if (newTeacher != null)
                {
                    var userService = _serviceProvider.GetRequiredService<UserService>();

                    try
                    {
                        var (login, password) = await userService.CreateTeacherAsync(
                            newTeacher.Name,
                            newTeacher.MiddleName,
                            newTeacher.LastName);

                        MessageBox.Show($"Преподаватель добавлен!\nЛогин: {login}\nПароль: {password}");

                        if (DataContext is AdminViewModel vm)
                        {
                            vm.Teachers.Add(newTeacher);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении преподавателя: {ex.Message}");
                    }
                }
            }
        }


    }
}
