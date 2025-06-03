using IntellectFlow.Views;
using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using IntellectFlow.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Windows;

namespace IntellectFlow
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            await AuthInitializer.EnsureAdminUser(_serviceProvider);

            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.AddDbContext<IntellectFlowDbContext>(options =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["defaultConnectionString"]?.ConnectionString;
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IntellectFlowDbContext>()
            .AddDefaultTokenProviders();

            services.AddSingleton<IServiceProvider>(sp => sp);

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddScoped<RoleService>();
            services.AddScoped<UserService>();
            services.AddScoped<AuthService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<AdminViewModel>();
            services.AddTransient<TeacherViewModel>();
            services.AddTransient<StudentViewModel>();
            services.AddSingleton<IUserContext, UserContext>();

            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AdminView>();
            services.AddTransient<TeacherView>();
            services.AddTransient<StudentView>();
        }
    }
}
