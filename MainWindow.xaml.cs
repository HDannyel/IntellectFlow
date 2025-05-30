using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace IntellectFlow.Views
{
    public partial class MainWindow : Window
    {
        private IServiceProvider _serviceProvider;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetContentForRole(string role)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider не инициализирован");

            RoleTextBlock.Text = $"Вы вошли как: {role}";

            switch (role)
            {
                case "Admin":
                    var adminView = _serviceProvider.GetRequiredService<AdminView>();
                    ContentControlArea.Content = adminView;
                    break;
                case "Teacher":
                    var teacherView = _serviceProvider.GetRequiredService<TeacherView>();
                    ContentControlArea.Content = teacherView;
                    break;
                default:
                    var studentView = _serviceProvider.GetRequiredService<StudentView>();
                    ContentControlArea.Content = studentView;
                    break;
            }
        }
    }
}
