using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            DataContext = _serviceProvider.GetRequiredService<AdminViewModel>();
        }

        private void DisciplinesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem is Discipline selectedDiscipline)
            {
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

        private async void DeleteTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminViewModel vm && vm.SelectedTeacher != null)
            {
                var teacher = vm.SelectedTeacher;

                var userService = _serviceProvider.GetRequiredService<UserService>();
                try
                {
                    await userService.DeleteUserAsync(teacher.Name); // Передаем логин пользователя

                    vm.Teachers.Remove(teacher);
                    vm.SelectedTeacher = null;

                    MessageBox.Show("Преподаватель успешно удален.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении преподавателя: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите преподавателя для удаления.");
            }
        }

        private async void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminViewModel vm && vm.SelectedStudent != null)
            {
                var student = vm.SelectedStudent;

                var userService = _serviceProvider.GetRequiredService<UserService>();
                try
                {
                    await userService.DeleteUserAsync(student.Name);

                    vm.Students.Remove(student);
                    vm.SelectedStudent = null;

                    MessageBox.Show("Студент успешно удален.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении студента: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для удаления.");
            }
        }

    }
}
