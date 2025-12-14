using System.Reactive.Linq;
using System.Reactive.Subjects;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 생산 서비스 구현
/// </summary>
public class ProductionService : IProductionService
{
    private readonly IApiClient _apiClient;
    private readonly Subject<Production> _productionSubject = new();

    public event EventHandler<Production>? ProductionRecorded;

    public ProductionService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<Production> RecordProductionAsync(ProductionInput input)
    {
        try
        {
            var result = await _apiClient.PostAsync<ProductionInput, Production>("/api/production", input);
            
            // 로컬 이벤트 발생 (데모)
            ProductionRecorded?.Invoke(this, result);
            _productionSubject.OnNext(result);
            
            return result;
        }
        catch
        {
            // 데모용 가짜 응답
            var production = new Production
            {
                Id = DateTime.Now.Ticks,
                WorkOrderNo = input.WorkOrderNo,
                EquipmentCode = input.EquipmentCode,
                LotNo = input.LotNo,
                Quantity = input.Quantity,
                DefectQuantity = input.DefectQuantity,
                CreatedAt = DateTime.Now
            };
            
            return production;
        }
    }

    public async Task<IEnumerable<Production>> GetProductionsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string? workOrderNo = null, 
        string? equipmentCode = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["fromDate"] = fromDate.ToString("yyyy-MM-dd"),
            ["toDate"] = toDate.ToString("yyyy-MM-dd")
        };

        if (!string.IsNullOrEmpty(workOrderNo))
            queryParams["workOrderNo"] = workOrderNo;

        if (!string.IsNullOrEmpty(equipmentCode))
            queryParams["equipmentCode"] = equipmentCode;

        try
        {
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}"));
            return await _apiClient.GetAsync<IEnumerable<Production>>("/api/production/history?" + queryString);
        }
        catch
        {
            return Enumerable.Empty<Production>();
        }
    }

    public async Task<DailyProductionSummary> GetTodaySummaryAsync(string? lineCode = null)
    {
        try
        {
            var url = "/api/production/today-summary";
            if (!string.IsNullOrEmpty(lineCode))
                url += $"?lineCode={lineCode}";
                
            return await _apiClient.GetAsync<DailyProductionSummary>(url);
        }
        catch
        {
            // 데모용 데이터
            return new DailyProductionSummary
            {
                Date = DateTime.Today,
                TotalPlanQuantity = 1000,
                TotalProducedQuantity = 850,
                TotalDefectQuantity = 12,
                WorkOrderCount = 5,
                CompletedWorkOrderCount = 3
            };
        }
    }

    public IObservable<Production> SubscribeToProduction(string? lineCode = null)
    {
        return _productionSubject.AsObservable();
    }
}
