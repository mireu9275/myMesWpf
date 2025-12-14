using CommunityToolkit.Mvvm.ComponentModel;
using MesClient.Core.Enums;

namespace MesClient.Core.Models;

/// <summary>
/// 품질 검사
/// </summary>
public partial class QualityInspection : BaseEntity
{
    [ObservableProperty]
    private string _inspectionNo = string.Empty;

    [ObservableProperty]
    private string _workOrderNo = string.Empty;

    [ObservableProperty]
    private string _lotNo = string.Empty;

    [ObservableProperty]
    private string _productCode = string.Empty;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _inspectionType = string.Empty;

    [ObservableProperty]
    private string _inspectionItem = string.Empty;

    [ObservableProperty]
    private string? _specification;

    [ObservableProperty]
    private double? _lowerLimit;

    [ObservableProperty]
    private double? _upperLimit;

    [ObservableProperty]
    private double? _measuredValue;

    [ObservableProperty]
    private string? _measuredValueText;

    [ObservableProperty]
    private QualityResult _result = QualityResult.NotInspected;

    [ObservableProperty]
    private DateTime _inspectionTime;

    [ObservableProperty]
    private string _inspectorId = string.Empty;

    [ObservableProperty]
    private string _inspectorName = string.Empty;

    [ObservableProperty]
    private string? _remarks;

    /// <summary>
    /// 규격 내 여부 확인
    /// </summary>
    public bool IsWithinSpec()
    {
        if (MeasuredValue == null || (LowerLimit == null && UpperLimit == null))
            return true;

        var value = MeasuredValue.Value;
        
        if (LowerLimit.HasValue && value < LowerLimit.Value)
            return false;
            
        if (UpperLimit.HasValue && value > UpperLimit.Value)
            return false;
            
        return true;
    }
}
