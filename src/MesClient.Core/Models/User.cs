using CommunityToolkit.Mvvm.ComponentModel;
using MesClient.Core.Enums;

namespace MesClient.Core.Models;

/// <summary>
/// 사용자 정보
/// </summary>
public partial class User : BaseEntity
{
    [ObservableProperty]
    private string _userId = string.Empty;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _department = string.Empty;

    [ObservableProperty]
    private UserRole _role = UserRole.Operator;

    [ObservableProperty]
    private DateTime? _lastLoginAt;

    [ObservableProperty]
    private string? _profileImageUrl;

    /// <summary>
    /// 특정 역할을 가지고 있는지 확인
    /// </summary>
    public bool HasRole(UserRole role) => (Role & role) == role;

    /// <summary>
    /// 관리자 권한 여부
    /// </summary>
    public bool IsAdmin => HasRole(UserRole.Admin);
}
