using MesClient.Core.Enums;
using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 품질 서비스 인터페이스
/// </summary>
public interface IQualityService
{
    /// <summary>
    /// 품질 검사 결과 등록
    /// </summary>
    Task<QualityInspection> RecordInspectionAsync(QualityInspectionInput input);

    /// <summary>
    /// 품질 검사 이력 조회
    /// </summary>
    Task<IEnumerable<QualityInspection>> GetInspectionsAsync(
        DateTime fromDate, 
        DateTime toDate,
        string? workOrderNo = null,
        QualityResult? result = null);

    /// <summary>
    /// LOT별 품질 검사 조회
    /// </summary>
    Task<IEnumerable<QualityInspection>> GetInspectionsByLotAsync(string lotNo);

    /// <summary>
    /// 품질 현황 요약
    /// </summary>
    Task<QualitySummary> GetQualitySummaryAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// 불량 유형별 통계
    /// </summary>
    Task<IDictionary<string, int>> GetDefectStatisticsAsync(DateTime fromDate, DateTime toDate);
}

/// <summary>
/// 품질 검사 입력
/// </summary>
public class QualityInspectionInput
{
    public string WorkOrderNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty;
    public string InspectionItem { get; set; } = string.Empty;
    public double? MeasuredValue { get; set; }
    public string? MeasuredValueText { get; set; }
    public QualityResult Result { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>
/// 품질 요약
/// </summary>
public class QualitySummary
{
    public int TotalInspections { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public double PassRate => TotalInspections > 0 
        ? Math.Round((double)PassCount / TotalInspections * 100, 2) 
        : 0;
}
