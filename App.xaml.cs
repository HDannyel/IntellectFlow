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


        private async Task InitializeDatabaseAndRolesAndAdmin()
        {
            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IntellectFlowDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
            await roleService.EnsureRolesCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                await userManager.DeleteAsync(adminUser); // удалим старого, если был битый
            }

            adminUser = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                Name = "System",
                LastName = "Administrator",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, "Admin@123");

            if (!createResult.Succeeded)
            {
                throw new Exception("Не удалось создать администратора: " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(adminUser, "Admin");

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

            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AdminView>();
            services.AddTransient<TeacherView>();
            services.AddTransient<StudentView>();
        }
    }
}
