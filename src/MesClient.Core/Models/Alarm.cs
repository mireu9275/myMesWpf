using CommunityToolkit.Mvvm.ComponentModel;

namespace MesClient.Core.Models;

/// <summary>
/// 알람/이벤트
/// </summary>
public partial class Alarm : BaseEntity
{
    [ObservableProperty]
    private string _alarmCode = string.Empty;

    [ObservableProperty]
    private string _alarmName = string.Empty;

    [ObservableProperty]
    private string _equipmentCode = string.Empty;

    [ObservableProperty]
    private string _equipmentName = string.Empty;

    [ObservableProperty]
    private AlarmLevel _level = AlarmLevel.Info;

    [ObservableProperty]
    private DateTime _occurredAt;

    [ObservableProperty]
    private DateTime? _acknowledgedAt;

    [ObservableProperty]
    private string? _acknowledgedBy;

    [ObservableProperty]
    private DateTime? _clearedAt;

    [ObservableProperty]
    private string? _clearedBy;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _action;

    /// <summary>
    /// 활성 알람 여부
    /// </summary>
    public new bool IsActive => ClearedAt == null;

    /// <summary>
    /// 확인된 알람 여부
    /// </summary>
    public bool IsAcknowledged => AcknowledgedAt != null;
}

/// <summary>
/// 알람 레벨
/// </summary>
public enum AlarmLevel
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}
