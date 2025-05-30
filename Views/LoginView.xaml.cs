using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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
                viewModel.LoginCommand.Execute(PasswordBox.Password);
            }
        }
    }
}
