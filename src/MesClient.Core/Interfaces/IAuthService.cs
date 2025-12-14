using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 인증 서비스 인터페이스
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 현재 로그인한 사용자
    /// </summary>
    User? CurrentUser { get; }

    /// <summary>
    /// 로그인 여부
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 로그인
    /// </summary>
    Task<AuthResult> LoginAsync(string userId, string password);

    /// <summary>
    /// 로그아웃
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// 비밀번호 변경
    /// </summary>
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);

    /// <summary>
    /// 토큰 갱신
    /// </summary>
    Task<bool> RefreshTokenAsync();

    /// <summary>
    /// 인증 상태 변경 이벤트
    /// </summary>
    event EventHandler<AuthChangedEventArgs>? AuthStateChanged;
}

/// <summary>
/// 인증 결과
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public User? User { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 인증 상태 변경 이벤트 인자
/// </summary>
public class AuthChangedEventArgs : EventArgs
{
    public bool IsAuthenticated { get; set; }
    public User? User { get; set; }
}
