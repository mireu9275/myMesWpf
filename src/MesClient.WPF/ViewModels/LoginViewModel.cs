using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Interfaces;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 로그인 ViewModel
/// </summary>
public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _userId = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe;

    [ObservableProperty]
    private string? _loginError;

    [ObservableProperty]
    private bool _isLoggingIn;

    public LoginViewModel(IAuthService authService, IDialogService dialogService)
    {
        _authService = authService;
        _dialogService = dialogService;
        Title = "로그인";
    }

    public override Task InitializeAsync()
    {
        // 저장된 사용자 ID 로드
        LoadSavedCredentials();
        return Task.CompletedTask;
    }

    private void LoadSavedCredentials()
    {
        // TODO: 설정에서 저장된 사용자 ID 로드
        // 예: UserId = Settings.Default.SavedUserId;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        if (IsLoggingIn) return;

        try
        {
            IsLoggingIn = true;
            LoginError = null;

            var result = await _authService.LoginAsync(UserId, Password);

            if (result.Success)
            {
                if (RememberMe)
                {
                    // TODO: 사용자 ID 저장
                }
                
                // 로그인 성공 - MainViewModel에서 네비게이션 처리
            }
            else
            {
                LoginError = result.ErrorMessage ?? "로그인에 실패했습니다.";
            }
        }
        catch (Exception ex)
        {
            LoginError = $"로그인 중 오류가 발생했습니다: {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(UserId) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsLoggingIn;
    }

    partial void OnUserIdChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string value)
    {
        LoginCommand.NotifyCanExecuteChanged();
    }
}
