using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using IntellectFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace IntellectFlow.Views
{
    public partial class StudentView : UserControl
    {
        public StudentView(IServiceProvider serviceProvider, IUserContext userContext)
        {
            InitializeComponent();

            if (userContext.StudentId == null)
                throw new InvalidOperationException("UserContext.StudentId is null for student user.");

            var context = serviceProvider.GetRequiredService<IntellectFlowDbContext>();
            DataContext = new StudentViewModel(context, userContext.StudentId.Value);
        }
        private void CoursesListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as StudentViewModel;

            if (viewModel?.SelectedCourse != null)
            {
                viewModel.OpenCourseDetailsCommand.Execute(null);
            }
        }

    }

}
