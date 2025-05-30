using System.Windows;
using IntellectFlow.DataModel;

namespace IntellectFlow.Views
{
    public partial class AddStudentWindow : Window
    {
        public Student NewStudent { get; private set; }

        public AddStudentWindow()
        {
            InitializeComponent();
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

            NewStudent = new Student
            {
                Name = NameTextBox.Text.Trim(),
                MiddleName = MidleNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim()
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
