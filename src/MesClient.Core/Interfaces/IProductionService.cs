using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 생산 서비스 인터페이스
/// </summary>
public interface IProductionService
{
    /// <summary>
    /// 생산 실적 등록
    /// </summary>
    Task<Production> RecordProductionAsync(ProductionInput input);

    /// <summary>
    /// 생산 실적 조회
    /// </summary>
    Task<IEnumerable<Production>> GetProductionsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string? workOrderNo = null,
        string? equipmentCode = null);

    /// <summary>
    /// 오늘의 생산 현황
    /// </summary>
    Task<DailyProductionSummary> GetTodaySummaryAsync(string? lineCode = null);

    /// <summary>
    /// 실시간 생산 현황 구독
    /// </summary>
    IObservable<Production> SubscribeToProduction(string? lineCode = null);

    /// <summary>
    /// 생산 실적 발생 이벤트
    /// </summary>
    event EventHandler<Production>? ProductionRecorded;
}

/// <summary>
/// 생산 실적 입력
/// </summary>
public class ProductionInput
{
    public string WorkOrderNo { get; set; } = string.Empty;
    public string EquipmentCode { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int DefectQuantity { get; set; }
    public string? DefectCode { get; set; }
    public string? DefectReason { get; set; }
}

/// <summary>
/// 일일 생산 요약
/// </summary>
public class DailyProductionSummary
{
    public DateTime Date { get; set; }
    public int TotalPlanQuantity { get; set; }
    public int TotalProducedQuantity { get; set; }
    public int TotalDefectQuantity { get; set; }
    public int WorkOrderCount { get; set; }
    public int CompletedWorkOrderCount { get; set; }
    public double AchievementRate => TotalPlanQuantity > 0 
        ? Math.Round((double)TotalProducedQuantity / TotalPlanQuantity * 100, 1) 
        : 0;
    public double DefectRate => TotalProducedQuantity > 0 
        ? Math.Round((double)TotalDefectQuantity / TotalProducedQuantity * 100, 2) 
        : 0;
}
