using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 메인 윈도우 ViewModel
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IAlarmService _alarmService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _currentViewName = "Dashboard";

    [ObservableProperty]
    private int _activeAlarmCount;

    [ObservableProperty]
    private bool _isMenuExpanded = true;

    public MainViewModel(
        IAuthService authService, 
        INavigationService navigationService,
        IAlarmService alarmService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _alarmService = alarmService;

        Title = "MES Client";

        // 인증 상태 변경 구독
        _authService.AuthStateChanged += OnAuthStateChanged;
        _navigationService.Navigated += OnNavigated;
    }

    public override async Task InitializeAsync()
    {
        // 초기 로그인 화면 표시
        if (!_authService.IsAuthenticated)
        {
            _navigationService.NavigateTo("Login");
        }
        else
        {
            CurrentUser = _authService.CurrentUser;
            IsLoggedIn = true;
            _navigationService.NavigateTo("Dashboard");
            await LoadActiveAlarmsAsync();
        }
    }

    private void OnAuthStateChanged(object? sender, AuthChangedEventArgs e)
    {
        IsLoggedIn = e.IsAuthenticated;
        CurrentUser = e.User;

        if (e.IsAuthenticated)
        {
            _navigationService.NavigateTo("Dashboard");
        }
        else
        {
            _navigationService.NavigateTo("Login");
        }
    }

    private void OnNavigated(object? sender, NavigationEventArgs e)
    {
        CurrentViewName = e.ViewName;
        if (e.ViewModel is ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }

    private async Task LoadActiveAlarmsAsync()
    {
        var alarms = await _alarmService.GetActiveAlarmsAsync();
        ActiveAlarmCount = alarms.Count();
    }

    [RelayCommand]
    private void NavigateTo(string viewName)
    {
        _navigationService.NavigateTo(viewName);
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
    }

    [RelayCommand]
    private void ToggleMenu()
    {
        IsMenuExpanded = !IsMenuExpanded;
    }

    [RelayCommand]
    private void ShowAlarms()
    {
        _navigationService.NavigateTo("Alarms");
    }

    [RelayCommand]
    private void ShowProfile()
    {
        _navigationService.NavigateTo("Settings");
    }
}
