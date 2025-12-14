namespace MesClient.Core.Interfaces;

/// <summary>
/// 네비게이션 서비스 인터페이스
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 현재 뷰 이름
    /// </summary>
    string CurrentView { get; }

    /// <summary>
    /// 뷰로 이동
    /// </summary>
    void NavigateTo(string viewName, object? parameter = null);

    /// <summary>
    /// 뒤로 이동
    /// </summary>
    bool GoBack();

    /// <summary>
    /// 뒤로 이동 가능 여부
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 네비게이션 이벤트
    /// </summary>
    event EventHandler<NavigationEventArgs>? Navigated;
}

/// <summary>
/// 네비게이션 이벤트 인자
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public string ViewName { get; set; } = string.Empty;
    public object? Parameter { get; set; }
}
