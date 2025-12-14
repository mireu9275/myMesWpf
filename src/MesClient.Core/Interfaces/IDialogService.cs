namespace MesClient.Core.Interfaces;

/// <summary>
/// 다이얼로그 서비스 인터페이스
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// 정보 메시지 표시
    /// </summary>
    Task ShowInfoAsync(string message, string title = "정보");

    /// <summary>
    /// 경고 메시지 표시
    /// </summary>
    Task ShowWarningAsync(string message, string title = "경고");

    /// <summary>
    /// 오류 메시지 표시
    /// </summary>
    Task ShowErrorAsync(string message, string title = "오류");

    /// <summary>
    /// 확인 다이얼로그 표시
    /// </summary>
    Task<bool> ShowConfirmAsync(string message, string title = "확인");

    /// <summary>
    /// 입력 다이얼로그 표시
    /// </summary>
    Task<string?> ShowInputAsync(string message, string title = "입력", string defaultValue = "");

    /// <summary>
    /// 커스텀 다이얼로그 표시
    /// </summary>
    Task<TResult?> ShowDialogAsync<TResult>(object viewModel);

    /// <summary>
    /// 로딩 표시
    /// </summary>
    IDisposable ShowLoading(string message = "처리중...");
}
