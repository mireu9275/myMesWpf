using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 품질 서비스 구현
/// </summary>
public class QualityService : IQualityService
{
    private readonly IApiClient _apiClient;

    public QualityService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<QualityInspection> RecordInspectionAsync(QualityInspectionInput input)
    {
        try
        {
            var result = await _apiClient.PostAsync<QualityInspectionInput, QualityInspection>("/api/quality/inspections", input);
            if (result == null)
                throw new InvalidOperationException("API 응답이 null입니다.");
            return result;
        }
        catch
        {
            // 데모용 응답
            return new QualityInspection
            {
                Id = DateTime.Now.Ticks,
                WorkOrderNo = input.WorkOrderNo,
                LotNo = input.LotNo,
                InspectionType = input.InspectionType,
                InspectionItem = input.InspectionItem,
                Result = input.Result,
                CreatedAt = DateTime.Now
            };
        }
    }

    public async Task<IEnumerable<QualityInspection>> GetInspectionsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string? workOrderNo = null, 
        QualityResult? result = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["fromDate"] = fromDate.ToString("yyyy-MM-dd"),
            ["toDate"] = toDate.ToString("yyyy-MM-dd")
        };

        if (!string.IsNullOrEmpty(workOrderNo))
            queryParams["workOrderNo"] = workOrderNo;

        if (result.HasValue)
            queryParams["result"] = result.Value.ToString();

        try
        {
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}"));
            return await _apiClient.GetAsync<IEnumerable<QualityInspection>>("/api/quality/inspections?" + queryString) ?? Enumerable.Empty<QualityInspection>();
        }
        catch
        {
            return Enumerable.Empty<QualityInspection>();
        }
    }

    public async Task<IEnumerable<QualityInspection>> GetInspectionsByLotAsync(string lotNo)
    {
        try
        {
            return await _apiClient.GetAsync<IEnumerable<QualityInspection>>($"/api/quality/inspections/lot/{lotNo}") ?? Enumerable.Empty<QualityInspection>();
        }
        catch
        {
            return Enumerable.Empty<QualityInspection>();
        }
    }

    public async Task<QualitySummary> GetQualitySummaryAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var queryParams = $"fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var result = await _apiClient.GetAsync<QualitySummary>($"/api/quality/summary?{queryParams}");
            if (result == null)
                throw new InvalidOperationException("API 응답이 null입니다.");
            return result;
        }
        catch
        {
            // 데모용 데이터
            return new QualitySummary
            {
                TotalInspections = 150,
                PassCount = 145,
                FailCount = 5
            };
        }
    }

    public async Task<IDictionary<string, int>> GetDefectStatisticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var queryParams = $"fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var result = await _apiClient.GetAsync<IDictionary<string, int>>($"/api/quality/defects/statistics?{queryParams}");
            if (result == null)
                throw new InvalidOperationException("API 응답이 null입니다.");
            return result;
        }
        catch
        {
            // 데모용 데이터
            return new Dictionary<string, int>
            {
                ["치수 불량"] = 3,
                ["스크래치"] = 2,
                ["오염"] = 1
            };
        }
    }
}
