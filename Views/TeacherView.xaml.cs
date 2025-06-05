using IntellectFlow.DataModel;
using IntellectFlow.Models;
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
    /// Логика взаимодействия для TeacherView.xaml
    /// </summary>
    public partial class TeacherView : UserControl
    {
        public TeacherView(IServiceProvider serviceProvider, int teacherId)
        {
            InitializeComponent();

            var context = serviceProvider.GetRequiredService<IntellectFlowDbContext>();
            DataContext = new TeacherViewModel(context, teacherId);
        }

        private void MyCourses_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is TeacherViewModel vm)
                vm.OpenCourseDetails();
        }


        private void CoursesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Логика при двойном клике по курсу
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem is Course selectedCourse)
            {
                // Например: открыть детали курса или редактировать
                MessageBox.Show($"Вы выбрали курс: {selectedCourse.Name}");
            }
        }
    }
}
