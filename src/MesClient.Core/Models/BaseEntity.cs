using CommunityToolkit.Mvvm.ComponentModel;

namespace MesClient.Core.Models;

/// <summary>
/// 모든 엔티티의 기본 클래스
/// </summary>
public abstract partial class BaseEntity : ObservableObject
{
    [ObservableProperty]
    private long _id;

    [ObservableProperty]
    private DateTime _createdAt;

    [ObservableProperty]
    private string _createdBy = string.Empty;

    [ObservableProperty]
    private DateTime? _modifiedAt;

    [ObservableProperty]
    private string? _modifiedBy;

    [ObservableProperty]
    private bool _isActive = true;
}
