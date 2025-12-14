using CommunityToolkit.Mvvm.ComponentModel;
using MesClient.Core.Interfaces;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// ViewModel 기본 클래스
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// 바쁘지 않은 상태
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// 초기화
    /// </summary>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 정리
    /// </summary>
    public virtual Task CleanupAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 작업 실행 (로딩 표시 포함)
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> action, string? errorMessage = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 작업 실행 (결과 반환)
    /// </summary>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> action, string? errorMessage = null)
    {
        if (IsBusy) return default;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            return await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
