using IntellectFlow.Helpers;
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
        private int GetCurrentTeacherId()
        {
            var userCtx = _serviceProvider.GetRequiredService<IUserContext>();
            return userCtx.UserId;          // ← сохраняйте при логине
        }



        public void SetContentForRole(string role)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider не инициализирован");

            RoleTextBlock.Text = $"Вы вошли как: {role}";

            if (role == "Teacher")
            {
                int teacherId = GetCurrentTeacherId(); // Получи текущий ID учителя (свой метод)
                ContentControlArea.Content = new TeacherView(_serviceProvider, teacherId);
            }
            else if (role == "Admin")
            {
                ContentControlArea.Content = _serviceProvider.GetRequiredService<AdminView>();
            }
            else
            {
                ContentControlArea.Content = _serviceProvider.GetRequiredService<StudentView>();
            }
        }

    }
}
