using MesClient.Core.Interfaces;
using MesClient.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MesClient.WPF.Services;

/// <summary>
/// 네비게이션 서비스 구현
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<string> _navigationStack = new();
    private string _currentView = string.Empty;

    public string CurrentView => _currentView;
    public bool CanGoBack => _navigationStack.Count > 1;

    public event EventHandler<NavigationEventArgs>? Navigated;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo(string viewName, object? parameter = null)
    {
        if (_currentView == viewName) return;

        var viewModel = CreateViewModel(viewName);
        if (viewModel != null)
        {
            _navigationStack.Push(viewName);
            _currentView = viewName;

            // MainViewModel에 현재 ViewModel 설정
            var mainViewModel = _serviceProvider.GetService<MainViewModel>();
            if (mainViewModel != null)
            {
                mainViewModel.CurrentViewModel = viewModel;
            }

            _ = viewModel.InitializeAsync();

            Navigated?.Invoke(this, new NavigationEventArgs 
            { 
                ViewName = viewName, 
                Parameter = parameter 
            });
        }
    }

    public bool GoBack()
    {
        if (!CanGoBack) return false;

        _navigationStack.Pop(); // 현재 화면 제거
        var previousView = _navigationStack.Peek();
        
        _currentView = string.Empty; // 중복 방지를 위해 초기화
        NavigateTo(previousView);
        
        return true;
    }

    private ViewModelBase? CreateViewModel(string viewName)
    {
        return viewName switch
        {
            "Login" => _serviceProvider.GetService<LoginViewModel>(),
            "Dashboard" => _serviceProvider.GetService<DashboardViewModel>(),
            "Production" => _serviceProvider.GetService<ProductionViewModel>(),
            "Equipment" => _serviceProvider.GetService<EquipmentViewModel>(),
            "Quality" => _serviceProvider.GetService<QualityViewModel>(),
            "Settings" => _serviceProvider.GetService<SettingsViewModel>(),
            _ => _serviceProvider.GetService<DashboardViewModel>()
        };
    }
}
