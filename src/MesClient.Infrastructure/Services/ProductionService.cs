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
            if (result == null)
                throw new InvalidOperationException("API 응답이 null입니다.");
            
            // 로컬 이벤트 발생 (데모)
            ProductionRecorded?.Invoke(this, result);
            _productionSubject.OnNext(result);
            
            return result;
        }
        catch
        {
            // 데모용 가짜 응답
            var now = DateTime.Now;
            var production = new Production
            {
                Id = now.Ticks,
                ProductionNo = $"PRD-{now:yyyyMMddHHmmss}",
                WorkOrderNo = input.WorkOrderNo,
                EquipmentCode = input.EquipmentCode,
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = input.LotNo,
                Quantity = input.Quantity,
                DefectQuantity = input.DefectQuantity,
                ProductionTime = now,
                OperatorId = "OP001",
                OperatorName = "김철수",
                CycleTime = 12.0,
                CreatedAt = now
            };
            
            // 로컬 이벤트 발생
            ProductionRecorded?.Invoke(this, production);
            _productionSubject.OnNext(production);
            
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
            var result = await _apiClient.GetAsync<IEnumerable<Production>>("/api/production/history?" + queryString);
            if (result != null)
            {
                return result;
            }
            
            // API 응답이 없는 경우 예시 데이터 반환
            return GetMockProductions(fromDate, toDate, workOrderNo, equipmentCode);
        }
        catch (Exception ex)
        {
            // 예시 데이터 반환
            try
            {
                return GetMockProductions(fromDate, toDate, workOrderNo, equipmentCode);
            }
            catch
            {
                return Enumerable.Empty<Production>();
            }
        }
    }

    private IEnumerable<Production> GetMockProductions(
        DateTime fromDate,
        DateTime toDate,
        string? workOrderNo = null,
        string? equipmentCode = null)
    {
        var now = DateTime.Now;
        var productions = new List<Production>
        {
            new Production
            {
                Id = 1,
                ProductionNo = "PRD-001",
                WorkOrderNo = "WO-2024-001",
                EquipmentCode = "EQ-001",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-001",
                Quantity = 100,
                DefectQuantity = 2,
                ProductionTime = now.AddHours(-2),
                OperatorId = "OP001",
                OperatorName = "김철수",
                CycleTime = 12.5,
                CreatedAt = now.AddHours(-2)
            },
            new Production
            {
                Id = 2,
                ProductionNo = "PRD-002",
                WorkOrderNo = "WO-2024-001",
                EquipmentCode = "EQ-001",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-002",
                Quantity = 150,
                DefectQuantity = 3,
                ProductionTime = now.AddHours(-1),
                OperatorId = "OP001",
                OperatorName = "김철수",
                CycleTime = 11.8,
                CreatedAt = now.AddHours(-1)
            },
            new Production
            {
                Id = 3,
                ProductionNo = "PRD-003",
                WorkOrderNo = "WO-2024-001",
                EquipmentCode = "EQ-001",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-003",
                Quantity = 120,
                DefectQuantity = 1,
                ProductionTime = now.AddMinutes(-30),
                OperatorId = "OP001",
                OperatorName = "김철수",
                CycleTime = 12.2,
                CreatedAt = now.AddMinutes(-30)
            },
            new Production
            {
                Id = 4,
                ProductionNo = "PRD-004",
                WorkOrderNo = "WO-2024-004",
                EquipmentCode = "EQ-003",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-004",
                Quantity = 200,
                DefectQuantity = 5,
                ProductionTime = now.AddHours(-3),
                OperatorId = "OP003",
                OperatorName = "박민수",
                CycleTime = 10.5,
                CreatedAt = now.AddHours(-3)
            },
            new Production
            {
                Id = 5,
                ProductionNo = "PRD-005",
                WorkOrderNo = "WO-2024-004",
                EquipmentCode = "EQ-003",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-005",
                Quantity = 180,
                DefectQuantity = 4,
                ProductionTime = now.AddHours(-2).AddMinutes(-30),
                OperatorId = "OP003",
                OperatorName = "박민수",
                CycleTime = 10.8,
                CreatedAt = now.AddHours(-2).AddMinutes(-30)
            },
            new Production
            {
                Id = 6,
                ProductionNo = "PRD-006",
                WorkOrderNo = "WO-2024-004",
                EquipmentCode = "EQ-003",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LotNo = "LOT-2024-006",
                Quantity = 70,
                DefectQuantity = 3,
                ProductionTime = now.AddMinutes(-15),
                OperatorId = "OP003",
                OperatorName = "박민수",
                CycleTime = 11.2,
                CreatedAt = now.AddMinutes(-15)
            }
        };

        if (!string.IsNullOrEmpty(workOrderNo))
        {
            productions = productions.Where(p => p.WorkOrderNo == workOrderNo).ToList();
        }

        if (!string.IsNullOrEmpty(equipmentCode))
        {
            productions = productions.Where(p => p.EquipmentCode == equipmentCode).ToList();
        }

        return productions.Where(p => p.ProductionTime >= fromDate && p.ProductionTime <= toDate);
    }

    public async Task<DailyProductionSummary> GetTodaySummaryAsync(string? lineCode = null)
    {
        try
        {
            var url = "/api/production/today-summary";
            if (!string.IsNullOrEmpty(lineCode))
                url += $"?lineCode={lineCode}";
                
            var result = await _apiClient.GetAsync<DailyProductionSummary>(url);
            if (result != null)
            {
                return result;
            }
        }
        catch
        {
            // 예외 발생 시 예시 데이터 반환
        }
        
        // API 응답이 없거나 실패한 경우 예시 데이터 반환
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

    public IObservable<Production> SubscribeToProduction(string? lineCode = null)
    {
        return _productionSubject.AsObservable();
    }
}
