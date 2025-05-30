using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using IntellectFlow.Helpers;

namespace IntellectFlow.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Передаём пароль вручную
                viewModel.LoginCommand.Execute(PasswordBox.Password);
            }
        }
    }
}
