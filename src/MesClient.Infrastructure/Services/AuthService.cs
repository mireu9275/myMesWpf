using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;
using Serilog;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 인증 서비스 구현
/// </summary>
public class AuthService : IAuthService
{
    private readonly IApiClient _apiClient;
    private readonly ILogger _logger;
    private User? _currentUser;
    private string? _accessToken;
    private string? _refreshToken;

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_accessToken);

    public event EventHandler<AuthChangedEventArgs>? AuthStateChanged;

    public AuthService(IApiClient apiClient, ILogger logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string userId, string password)
    {
        try
        {
            _logger.Information("로그인 시도: {UserId}", userId);

            var request = new { UserId = userId, Password = password };
            var response = await _apiClient.PostAsync<object, LoginResponse>("/api/auth/login", request);

            if (response?.Success == true && response.User != null)
            {
                _currentUser = response.User;
                _accessToken = response.Token;
                _refreshToken = response.RefreshToken;
                
                _apiClient.SetAccessToken(_accessToken);
                
                _logger.Information("로그인 성공: {UserId}", userId);
                
                AuthStateChanged?.Invoke(this, new AuthChangedEventArgs 
                { 
                    IsAuthenticated = true, 
                    User = _currentUser 
                });

                return new AuthResult
                {
                    Success = true,
                    User = _currentUser,
                    Token = _accessToken,
                    RefreshToken = _refreshToken,
                    ExpiresAt = response.ExpiresAt
                };
            }

            return new AuthResult
            {
                Success = false,
                ErrorMessage = response?.Message ?? "로그인에 실패했습니다."
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "로그인 오류: {UserId}", userId);
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "로그인 중 오류가 발생했습니다."
            };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            if (IsAuthenticated)
            {
                await _apiClient.PostAsync<object, object>("/api/auth/logout", new { });
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "로그아웃 API 호출 실패");
        }
        finally
        {
            _currentUser = null;
            _accessToken = null;
            _refreshToken = null;
            _apiClient.SetAccessToken(null);
            
            AuthStateChanged?.Invoke(this, new AuthChangedEventArgs 
            { 
                IsAuthenticated = false, 
                User = null 
            });
            
            _logger.Information("로그아웃 완료");
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            var request = new { CurrentPassword = currentPassword, NewPassword = newPassword };
            var response = await _apiClient.PostAsync<object, ApiResponse<bool>>("/api/auth/change-password", request);
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "비밀번호 변경 실패");
            return false;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_refreshToken))
                return false;

            var request = new { RefreshToken = _refreshToken };
            var response = await _apiClient.PostAsync<object, LoginResponse>("/api/auth/refresh", request);

            if (response?.Success == true)
            {
                _accessToken = response.Token;
                _refreshToken = response.RefreshToken;
                _apiClient.SetAccessToken(_accessToken);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "토큰 갱신 실패");
            return false;
        }
    }

    private class LoginResponse
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Message { get; set; }
    }
}
