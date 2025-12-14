using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.WPF.Models;
using MaterialDesignThemes.Wpf;

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

    [ObservableProperty]
    private ObservableCollection<MenuGroup> _menuGroups = new();

    [ObservableProperty]
    private MenuItem? _selectedMenuItem;

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

        // 메뉴 구조 초기화
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        // 자재관리
        var materialGroup = new MenuGroup("자재관리", PackIconKind.PackageVariant)
        {
            Items =
            {
                new MenuItem("자재 입고", "MaterialInbound", PackIconKind.ArrowDownBold),
                new MenuItem("자재 출고", "MaterialOutbound", PackIconKind.ArrowUpBold),
                new MenuItem("자재 조회", "MaterialSearch", PackIconKind.Magnify)
            }
        };

        // 생산관리
        var productionGroup = new MenuGroup("생산관리", PackIconKind.Factory)
        {
            Items =
            {
                new MenuItem("작업지시", "Production", PackIconKind.ClipboardText),
                new MenuItem("생산 실적", "ProductionRecord", PackIconKind.ChartLine)
            }
        };

        // 설비관리
        var equipmentGroup = new MenuGroup("설비관리", PackIconKind.Factory)
        {
            Items =
            {
                new MenuItem("설비 현황", "Equipment", PackIconKind.Monitor),
                new MenuItem("설비 정비", "EquipmentMaintenance", PackIconKind.Wrench)
            }
        };

        // 품질관리
        var qualityGroup = new MenuGroup("품질관리", PackIconKind.CheckCircle)
        {
            Items =
            {
                new MenuItem("품질 검사", "Quality", PackIconKind.ClipboardCheck),
                new MenuItem("불량 관리", "DefectManagement", PackIconKind.AlertCircle)
            }
        };

        // 제품관리
        var productGroup = new MenuGroup("제품관리", PackIconKind.Box)
        {
            Items =
            {
                new MenuItem("제품 정보", "ProductInfo", PackIconKind.Information),
                new MenuItem("BOM 관리", "BomManagement", PackIconKind.FileTree)
            }
        };

        MenuGroups = new ObservableCollection<MenuGroup>
        {
            materialGroup,
            productionGroup,
            equipmentGroup,
            qualityGroup,
            productGroup
        };
    }

    public override async Task InitializeAsync()
    {
        try
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Main 초기화 오류: {ex.Message}");
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
        CurrentViewName = GetViewDisplayName(e.ViewName);
        if (e.ViewModel is ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }

    private string GetViewDisplayName(string viewName)
    {
        return viewName switch
        {
            "Dashboard" => "대시보드",
            "Production" => "작업지시",
            "Equipment" => "설비 현황",
            "Quality" => "품질 검사",
            "Settings" => "설정",
            "MaterialInbound" => "자재 입고",
            "MaterialOutbound" => "자재 출고",
            "MaterialSearch" => "자재 조회",
            "ProductionRecord" => "생산 실적",
            "EquipmentMaintenance" => "설비 정비",
            "DefectManagement" => "불량 관리",
            "ProductInfo" => "제품 정보",
            "BomManagement" => "BOM 관리",
            _ => viewName
        };
    }

    private async Task LoadActiveAlarmsAsync()
    {
        try
        {
            var alarms = await _alarmService.GetActiveAlarmsAsync();
            ActiveAlarmCount = alarms.Count();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"알람 로드 오류: {ex.Message}");
            ActiveAlarmCount = 0;
        }
    }

    [RelayCommand]
    private void NavigateTo(string viewName)
    {
        _navigationService.NavigateTo(viewName);
        
        // 선택된 메뉴 아이템 업데이트
        foreach (var group in MenuGroups)
        {
            foreach (var item in group.Items)
            {
                item.IsSelected = item.ViewName == viewName;
                if (item.IsSelected)
                {
                    SelectedMenuItem = item;
                    group.IsSelected = true;
                    group.IsExpanded = true;
                }
            }
        }
    }

    partial void OnSelectedMenuItemChanged(MenuItem? value)
    {
        if (value != null)
        {
            NavigateTo(value.ViewName);
        }
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

    [RelayCommand]
    private void NavigateToGroup(MenuGroup group)
    {
        group.IsExpanded = !group.IsExpanded;
    }
}
