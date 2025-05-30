using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

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

            ContentControlArea.Content = role switch
            {
                "Admin" => _serviceProvider.GetRequiredService<AdminView>(),
                "Teacher" => _serviceProvider.GetRequiredService<TeacherView>(),
                _ => _serviceProvider.GetRequiredService<StudentView>(),
            };
        }
    }
}
