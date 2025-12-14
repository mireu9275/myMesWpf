using System.Windows;
using MesClient.Core.Interfaces;
using MaterialDesignThemes.Wpf;

namespace MesClient.WPF.Services;

/// <summary>
/// 다이얼로그 서비스 구현
/// </summary>
public class DialogService : IDialogService
{
    public Task ShowInfoAsync(string message, string title = "정보")
    {
        return ShowMessageBoxAsync(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public Task ShowWarningAsync(string message, string title = "경고")
    {
        return ShowMessageBoxAsync(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public Task ShowErrorAsync(string message, string title = "오류")
    {
        return ShowMessageBoxAsync(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public Task<bool> ShowConfirmAsync(string message, string title = "확인")
    {
        var result = MessageBox.Show(
            message, 
            title, 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);
        
        return Task.FromResult(result == MessageBoxResult.Yes);
    }

    public Task<string?> ShowInputAsync(string message, string title = "입력", string defaultValue = "")
    {
        // 간단한 입력 다이얼로그 (실제로는 커스텀 다이얼로그 사용 권장)
        var result = Microsoft.VisualBasic.Interaction.InputBox(message, title, defaultValue);
        return Task.FromResult(string.IsNullOrEmpty(result) ? null : result);
    }

    public Task<TResult?> ShowDialogAsync<TResult>(object viewModel)
    {
        // TODO: Material Design DialogHost 사용하여 커스텀 다이얼로그 구현
        throw new NotImplementedException("커스텀 다이얼로그는 추후 구현 예정입니다.");
    }

    public IDisposable ShowLoading(string message = "처리중...")
    {
        // TODO: 로딩 오버레이 구현
        return new LoadingDisposable();
    }

    private Task ShowMessageBoxAsync(string message, string title, MessageBoxButton button, MessageBoxImage icon)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, button, icon);
        });
        return Task.CompletedTask;
    }

    private class LoadingDisposable : IDisposable
    {
        public void Dispose()
        {
            // 로딩 오버레이 닫기
        }
    }
}
