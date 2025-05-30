using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace IntellectFlow.Views
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        // Конструктор по умолчанию для WPF (нужен для XAML)
        public MainWindow() : this(null)
        {
        }

        // Основной конструктор с DI
        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        public void SetContentForRole(string role)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider не инициализирован");

            switch (role)
            {
                case "Admin":
                    var adminView = _serviceProvider.GetRequiredService<AdminView>();
                    Content = adminView;
                    break;
                case "Teacher":
                    var teacherView = _serviceProvider.GetRequiredService<TeacherView>();
                    Content = teacherView;
                    break;
                default:
                    var studentView = _serviceProvider.GetRequiredService<StudentView>();
                    Content = studentView;
                    break;
            }
        }
    }
}
