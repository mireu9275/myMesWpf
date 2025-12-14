using CommunityToolkit.Mvvm.ComponentModel;

namespace MesClient.Core.Models;

/// <summary>
/// 생산 실적
/// </summary>
public partial class Production : BaseEntity
{
    [ObservableProperty]
    private string _productionNo = string.Empty;

    [ObservableProperty]
    private string _workOrderNo = string.Empty;

    [ObservableProperty]
    private string _equipmentCode = string.Empty;

    [ObservableProperty]
    private string _productCode = string.Empty;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _lotNo = string.Empty;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private int _defectQuantity;

    [ObservableProperty]
    private DateTime _productionTime;

    [ObservableProperty]
    private string _operatorId = string.Empty;

    [ObservableProperty]
    private string _operatorName = string.Empty;

    [ObservableProperty]
    private double _cycleTime;

    [ObservableProperty]
    private string? _defectCode;

    [ObservableProperty]
    private string? _defectReason;

    /// <summary>
    /// 양품수량
    /// </summary>
    public int GoodQuantity => Quantity - DefectQuantity;
}
