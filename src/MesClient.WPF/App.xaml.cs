using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MesClient.Core.Interfaces;
using MesClient.Infrastructure.Api;
using MesClient.Infrastructure.Configuration;
using MesClient.Infrastructure.Services;
using MesClient.WPF.Services;
using MesClient.WPF.ViewModels;
using Serilog;

namespace MesClient.WPF;

/// <summary>
/// App.xaml에 대한 상호작용 로직
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Debug()
                    .WriteTo.File("logs/mes-.log", 
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30)
                    .WriteTo.Debug();
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services);
            })
            .Build();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        services.AddSingleton<AppSettings>();

        // HTTP Client
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Core Services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IWorkOrderService, WorkOrderService>();
        services.AddSingleton<IEquipmentService, EquipmentService>();
        services.AddSingleton<IAlarmService, AlarmService>();
        services.AddSingleton<IProductionService, ProductionService>();
        services.AddSingleton<IQualityService, QualityService>();

        // WPF Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ProductionViewModel>();
        services.AddTransient<EquipmentViewModel>();
        services.AddTransient<QualityViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Views
        services.AddTransient<Views.MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<Views.MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
        }

        base.OnExit(e);
    }

    /// <summary>
    /// 서비스 프로바이더
    /// </summary>
    public static IServiceProvider Services => ((App)Current)._host.Services;
}
