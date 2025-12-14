using CommunityToolkit.Mvvm.ComponentModel;

namespace MesClient.Core.Models;

/// <summary>
/// 제품 정보
/// </summary>
public partial class Product : BaseEntity
{
    [ObservableProperty]
    private string _productCode = string.Empty;

    [ObservableProperty]
    private string _productName = string.Empty;

    [ObservableProperty]
    private string _productType = string.Empty;

    [ObservableProperty]
    private string? _specification;

    [ObservableProperty]
    private string _unit = "EA";

    [ObservableProperty]
    private double _standardCycleTime;

    [ObservableProperty]
    private string? _customerCode;

    [ObservableProperty]
    private string? _customerName;

    [ObservableProperty]
    private string? _drawingNo;

    [ObservableProperty]
    private string? _remarks;
}
