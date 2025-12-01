using Avia.Data;
using Avia.Infrastructure;
using Avia.Services;
using Avia.Services.Interfaces;
using Avia.ViewModels;
using Avia.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.IO;
using System.Linq;
using System.Windows;

namespace Avia;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Регистрируем ENUM типы PostgreSQL ДО создания подключений
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<Avia.Data.Entities.RoleType>("avia.role_type");
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<Avia.Data.Entities.ClassType>("avia.class_type");
            Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<Avia.Data.Entities.TicketStatus>("avia.ticket_status");

            // Build configuration
            var basePath = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(basePath, "appsettings.json");
            
            System.Diagnostics.Debug.WriteLine($"Looking for appsettings.json at: {configPath}");
            System.Diagnostics.Debug.WriteLine($"File exists: {File.Exists(configPath)}");
            
            if (!File.Exists(configPath))
            {
                // Пробуем найти в директории приложения
                var appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                configPath = Path.Combine(appBasePath, "appsettings.json");
                System.Diagnostics.Debug.WriteLine($"Trying app base directory: {configPath}");
                System.Diagnostics.Debug.WriteLine($"File exists: {File.Exists(configPath)}");
                
                if (File.Exists(configPath))
                {
                    basePath = appBasePath;
                }
            }
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            AppConfigurationManager.Initialize(configuration);
            System.Diagnostics.Debug.WriteLine("Configuration loaded successfully");

            // Setup DI
            System.Diagnostics.Debug.WriteLine("Configuring services...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            System.Diagnostics.Debug.WriteLine("Building service provider...");
            _serviceProvider = serviceCollection.BuildServiceProvider();
            System.Diagnostics.Debug.WriteLine("Service provider built");

            // Initialize database (не блокируем UI)
            Task.Run(() => InitializeDatabase());

            // Show login window
            System.Diagnostics.Debug.WriteLine("Creating LoginViewModel...");
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            System.Diagnostics.Debug.WriteLine("LoginViewModel created");
            
            System.Diagnostics.Debug.WriteLine("Creating LoginView...");
            var loginWindow = _serviceProvider.GetRequiredService<LoginView>();
            System.Diagnostics.Debug.WriteLine("LoginView created");
            
            loginWindow.DataContext = loginViewModel;
            MainWindow = loginWindow; // Устанавливаем как главное окно
            
            System.Diagnostics.Debug.WriteLine("Showing login window...");
            loginWindow.Show();
            System.Diagnostics.Debug.WriteLine("Login window shown");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Критическая ошибка при запуске приложения:\n\n{ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nДетали: {ex.InnerException.Message}";
            }
            errorMessage += $"\n\nStack Trace:\n{ex.StackTrace}";
            
            MessageBox.Show(
                errorMessage,
                "Ошибка запуска приложения",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            // Завершаем приложение при критической ошибке
            Shutdown();
        }
    }

    private async Task InitializeDatabase()
    {
        try
        {
            if (_serviceProvider == null)
            {
                System.Diagnostics.Debug.WriteLine("ServiceProvider is null!");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AviaDbContext>();
            var connectionString = AppConfigurationManager.GetConnectionString();

            System.Diagnostics.Debug.WriteLine($"Initializing database with connection: {connectionString}");

            var initializer = new DatabaseInitializer(context, connectionString);
            await initializer.InitializeAsync();
            
            System.Diagnostics.Debug.WriteLine("Database initialized successfully");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Ошибка инициализации базы данных: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nДетали: {ex.InnerException.Message}";
            }
            
            errorMessage += "\n\nУбедитесь, что:\n" +
                "1. PostgreSQL сервер запущен\n" +
                "2. Строка подключения в appsettings.json корректна\n" +
                "3. База данных доступна\n" +
                "4. Файл DBCreate.sql находится в корне проекта";
            
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {errorMessage}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Показываем ошибку в UI потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    errorMessage,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            });
            
            // Приложение продолжит работу даже при ошибке БД
            // Пользователь сможет попробовать подключиться позже
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        try
        {
            // Database
            System.Diagnostics.Debug.WriteLine("Getting connection string...");
            var connectionString = AppConfigurationManager.GetConnectionString();
            System.Diagnostics.Debug.WriteLine($"Connection string obtained: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");
            
            // Добавляем установку search_path в строку подключения
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
            builder.SearchPath = "avia";
            var finalConnectionString = builder.ConnectionString;
            
            services.AddDbContext<AviaDbContext>(options =>
            {
                options.UseNpgsql(finalConnectionString);
                options.AddInterceptors(new Data.SearchPathCommandInterceptor());
            });
            System.Diagnostics.Debug.WriteLine("DbContext registered");

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<ITicketService, TicketService>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegistrationViewModel>();
        services.AddTransient<AdminMainViewModel>();
        services.AddTransient<ClientMainViewModel>();
        services.AddTransient<AdminUserEditViewModel>();
        services.AddTransient<AdminFlightEditViewModel>();
        services.AddTransient<BuyTicketViewModel>();
        services.AddTransient<PersonalCabinetViewModel>();

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<RegistrationView>();
        services.AddTransient<AdminMainView>();
        services.AddTransient<ClientMainView>();
        services.AddTransient<AdminUserEditView>();
        services.AddTransient<AdminFlightEditView>();
        services.AddTransient<BuyTicketView>();
        services.AddTransient<PersonalCabinetView>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ConfigureServices: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}

