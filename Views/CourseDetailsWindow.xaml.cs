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
    /// Логика взаимодействия для CourseDetailsWindow.xaml
    /// </summary>
    public partial class CourseDetailsWindow : Window
    {
        public CourseDetailsWindow()
        {
            InitializeComponent();
        }
        private void Students_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CourseDetailsViewModel viewModel && viewModel.SelectedStudentInCourse != null)
            {
                viewModel.OpenStudentTasks(viewModel.SelectedStudentInCourse);
            }
        }

    }
}
