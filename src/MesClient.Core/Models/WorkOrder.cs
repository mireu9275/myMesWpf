using CommunityToolkit.Mvvm.ComponentModel;
using MesClient.Core.Enums;

namespace MesClient.Core.Models;

/// <summary>
/// 작업지시
/// </summary>
public partial class WorkOrder : BaseEntity
{
    [ObservableProperty]
    private string _workOrderNo = string.Empty;

    [ObservableProperty]
    private string _productCode = string.Empty;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _lineCode = string.Empty;

    [ObservableProperty]
    private string _lineName = string.Empty;

    [ObservableProperty]
    private string? _equipmentCode;

    [ObservableProperty]
    private string? _equipmentName;

    [ObservableProperty]
    private int _planQuantity;

    [ObservableProperty]
    private int _producedQuantity;

    [ObservableProperty]
    private int _defectQuantity;

    [ObservableProperty]
    private WorkOrderStatus _status = WorkOrderStatus.Planned;

    [ObservableProperty]
    private DateTime _planStartTime;

    [ObservableProperty]
    private DateTime _planEndTime;

    [ObservableProperty]
    private DateTime? _actualStartTime;

    [ObservableProperty]
    private DateTime? _actualEndTime;

    [ObservableProperty]
    private int _priority = 5;

    [ObservableProperty]
    private string? _remarks;

    /// <summary>
    /// 진행률 (%)
    /// </summary>
    public double ProgressRate => PlanQuantity > 0 
        ? Math.Round((double)ProducedQuantity / PlanQuantity * 100, 1) 
        : 0;

    /// <summary>
    /// 불량률 (%)
    /// </summary>
    public double DefectRate => ProducedQuantity > 0 
        ? Math.Round((double)DefectQuantity / ProducedQuantity * 100, 2) 
        : 0;

    /// <summary>
    /// 양품수량
    /// </summary>
    public int GoodQuantity => ProducedQuantity - DefectQuantity;
}
