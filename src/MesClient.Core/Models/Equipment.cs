using CommunityToolkit.Mvvm.ComponentModel;
using MesClient.Core.Enums;

namespace MesClient.Core.Models;

/// <summary>
/// 설비 정보
/// </summary>
public partial class Equipment : BaseEntity
{
    [ObservableProperty]
    private string _equipmentCode = string.Empty;

    [ObservableProperty]
    private string _equipmentName = string.Empty;

    [ObservableProperty]
    private string _lineCode = string.Empty;

    [ObservableProperty]
    private string _lineName = string.Empty;

    [ObservableProperty]
    private string _processCode = string.Empty;

    [ObservableProperty]
    private string _processName = string.Empty;

    [ObservableProperty]
    private EquipmentStatus _status = EquipmentStatus.Offline;

    [ObservableProperty]
    private string? _currentWorkOrderNo;

    [ObservableProperty]
    private double _utilizationRate;

    [ObservableProperty]
    private DateTime? _lastMaintenanceDate;

    [ObservableProperty]
    private DateTime? _nextMaintenanceDate;

    [ObservableProperty]
    private string? _operatorId;

    [ObservableProperty]
    private string? _operatorName;

    /// <summary>
    /// 설비 가동 가능 여부
    /// </summary>
    public bool IsAvailable => Status == EquipmentStatus.Idle || Status == EquipmentStatus.Running;
}
