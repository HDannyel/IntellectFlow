using System.Windows;
using IntellectFlow.DataModel;
using System.Collections.Generic;
using System.Linq;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.Views
{
    public partial class AddStudentWindow : Window
    {
        public Student NewStudent { get; private set; }
        public Group SelectedGroup { get; private set; }
        private readonly IntellectFlowDbContext _dbContext;

        public AddStudentWindow(IntellectFlowDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadGroups();
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(MidleNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (GroupComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите группу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedGroup = (Group)GroupComboBox.SelectedItem;

            // Если группа не отслеживается контекстом — добавить
            if (_dbContext.Entry(selectedGroup).State == EntityState.Detached)
            {
                _dbContext.Groups.Attach(selectedGroup);
            }

            // Создание нового студента
            NewStudent = new Student
            {
                Name = NameTextBox.Text.Trim(),
                MiddleName = MidleNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                StudentGroups = new List<StudentGroup>
        {
            new StudentGroup
            {
                Group = selectedGroup
            }
        }
            };

            // Сохраняем студента и связь
            _dbContext.Students.Add(NewStudent);
            _dbContext.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void LoadGroups()
        {
            var groups = _dbContext.Groups.ToList();
            GroupComboBox.ItemsSource = groups;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}