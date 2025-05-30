using IntellectFlow.DataModel;
using IntellectFlow.Helpers;
using IntellectFlow.Models;
using IntellectFlow.ViewModels;
using IntellectFlow.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IntellectFlow
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public IServiceProvider ServiceProvider => _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceProvider = ServiceProvider;

            // Инициализация администратора
            await AuthInitializer.EnsureAdminUser(serviceProvider);

            // Правильно создавать LoginView через DI:
            var loginView = serviceProvider.GetRequiredService<LoginView>();
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
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["defaultConnectionString"]?.ConnectionString;
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
            services.AddTransient<AdminView>(sp => new AdminView(sp));

            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AdminView>();
            services.AddTransient<TeacherView>();
            services.AddTransient<StudentView>();
        }
    }
}
