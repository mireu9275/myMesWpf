using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 설정 ViewModel
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private User? _currentUser;

    // 테마 설정
    [ObservableProperty]
    private bool _isDarkTheme;

    // 알림 설정
    [ObservableProperty]
    private bool _enableNotifications = true;

    [ObservableProperty]
    private bool _enableSoundAlerts = true;

    [ObservableProperty]
    private bool _enableDesktopAlerts = true;

    // 새로고침 설정
    [ObservableProperty]
    private int _refreshInterval = 10;

    // 언어 설정
    [ObservableProperty]
    private string _selectedLanguage = "ko-KR";

    [ObservableProperty]
    private string[] _availableLanguages = new[] { "ko-KR", "en-US", "ja-JP", "zh-CN" };

    // API 설정
    [ObservableProperty]
    private string _apiBaseUrl = "http://localhost:5000";

    [ObservableProperty]
    private int _apiTimeout = 30;

    public SettingsViewModel(
        IAuthService authService,
        IDialogService dialogService)
    {
        _authService = authService;
        _dialogService = dialogService;

        Title = "설정";
        CurrentUser = authService.CurrentUser;
    }

    public override Task InitializeAsync()
    {
        LoadSettings();
        return Task.CompletedTask;
    }

    private void LoadSettings()
    {
        // TODO: 저장된 설정 로드
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        try
        {
            // TODO: 설정 저장
            await _dialogService.ShowInfoAsync("설정이 저장되었습니다.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync($"설정 저장 실패: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        var currentPassword = await _dialogService.ShowInputAsync("현재 비밀번호를 입력하세요:");
        if (string.IsNullOrWhiteSpace(currentPassword)) return;

        var newPassword = await _dialogService.ShowInputAsync("새 비밀번호를 입력하세요:");
        if (string.IsNullOrWhiteSpace(newPassword)) return;

        var confirmPassword = await _dialogService.ShowInputAsync("새 비밀번호를 다시 입력하세요:");
        if (newPassword != confirmPassword)
        {
            await _dialogService.ShowWarningAsync("비밀번호가 일치하지 않습니다.");
            return;
        }

        var success = await _authService.ChangePasswordAsync(currentPassword, newPassword);
        if (success)
        {
            await _dialogService.ShowInfoAsync("비밀번호가 변경되었습니다.");
        }
        else
        {
            await _dialogService.ShowErrorAsync("비밀번호 변경에 실패했습니다.");
        }
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        IsDarkTheme = false;
        EnableNotifications = true;
        EnableSoundAlerts = true;
        EnableDesktopAlerts = true;
        RefreshInterval = 10;
        SelectedLanguage = "ko-KR";
        ApiBaseUrl = "http://localhost:5000";
        ApiTimeout = 30;
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        try
        {
            IsBusy = true;
            // TODO: API 연결 테스트
            await Task.Delay(1000); // 시뮬레이션
            await _dialogService.ShowInfoAsync("서버 연결이 정상입니다.");
        }
        catch
        {
            await _dialogService.ShowErrorAsync("서버에 연결할 수 없습니다.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
