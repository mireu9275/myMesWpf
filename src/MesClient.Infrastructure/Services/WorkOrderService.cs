using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;
using Serilog;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 작업지시 서비스 구현
/// </summary>
public class WorkOrderService : IWorkOrderService
{
    private readonly IApiClient _apiClient;
    private readonly ILogger _logger;

    public WorkOrderService(IApiClient apiClient, ILogger logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<WorkOrder>> GetWorkOrdersAsync(
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        WorkOrderStatus? status = null,
        string? lineCode = null)
    {
        try
        {
            var queryParams = new List<string>();
            
            if (fromDate.HasValue)
                queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            if (toDate.HasValue)
                queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");
            if (status.HasValue)
                queryParams.Add($"status={(int)status.Value}");
            if (!string.IsNullOrEmpty(lineCode))
                queryParams.Add($"lineCode={lineCode}");

            var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _apiClient.GetAsync<ApiResponse<IEnumerable<WorkOrder>>>($"/api/workorders{query}");
            
            if (response?.Success == true && response.Data != null)
            {
                return response.Data;
            }
            
            // API 응답이 없거나 실패한 경우 예시 데이터 반환
            return GetMockWorkOrders(fromDate, toDate, status, lineCode);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 목록 조회 실패");
            // 예시 데이터 반환
            try
            {
                return GetMockWorkOrders(fromDate, toDate, status, lineCode);
            }
            catch (Exception mockEx)
            {
                _logger.Error(mockEx, "예시 데이터 생성 실패");
                return Enumerable.Empty<WorkOrder>();
            }
        }
    }

    private IEnumerable<WorkOrder> GetMockWorkOrders(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        WorkOrderStatus? status = null,
        string? lineCode = null)
    {
        var now = DateTime.Now;
        var workOrders = new List<WorkOrder>
        {
            new WorkOrder
            {
                Id = 1,
                WorkOrderNo = "WO-2024-001",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LineCode = "LINE-01",
                LineName = "1라인",
                EquipmentCode = "EQ-001",
                EquipmentName = "사출기 1호",
                PlanQuantity = 1000,
                ProducedQuantity = 750,
                DefectQuantity = 15,
                Status = WorkOrderStatus.InProgress,
                PlanStartTime = now.Date.AddHours(8),
                PlanEndTime = now.Date.AddHours(17),
                ActualStartTime = now.Date.AddHours(8),
                Priority = 1,
                CreatedAt = now.AddDays(-1)
            },
            new WorkOrder
            {
                Id = 2,
                WorkOrderNo = "WO-2024-002",
                ProductCode = "PRD-002",
                ProductName = "스마트폰 케이스 B",
                LineCode = "LINE-01",
                LineName = "1라인",
                EquipmentCode = "EQ-002",
                EquipmentName = "사출기 2호",
                PlanQuantity = 800,
                ProducedQuantity = 800,
                DefectQuantity = 8,
                Status = WorkOrderStatus.Completed,
                PlanStartTime = now.Date.AddDays(-1).AddHours(8),
                PlanEndTime = now.Date.AddDays(-1).AddHours(17),
                ActualStartTime = now.Date.AddDays(-1).AddHours(8),
                ActualEndTime = now.Date.AddDays(-1).AddHours(16),
                Priority = 2,
                CreatedAt = now.AddDays(-2)
            },
            new WorkOrder
            {
                Id = 3,
                WorkOrderNo = "WO-2024-003",
                ProductCode = "PRD-003",
                ProductName = "배터리 케이스",
                LineCode = "LINE-02",
                LineName = "2라인",
                EquipmentCode = "EQ-003",
                EquipmentName = "조립기 1호",
                PlanQuantity = 500,
                ProducedQuantity = 0,
                DefectQuantity = 0,
                Status = WorkOrderStatus.Waiting,
                PlanStartTime = now.Date.AddHours(9),
                PlanEndTime = now.Date.AddHours(18),
                Priority = 3,
                CreatedAt = now.AddHours(-2)
            },
            new WorkOrder
            {
                Id = 4,
                WorkOrderNo = "WO-2024-004",
                ProductCode = "PRD-001",
                ProductName = "스마트폰 케이스 A",
                LineCode = "LINE-02",
                LineName = "2라인",
                EquipmentCode = "EQ-004",
                EquipmentName = "조립기 2호",
                PlanQuantity = 1200,
                ProducedQuantity = 450,
                DefectQuantity = 12,
                Status = WorkOrderStatus.InProgress,
                PlanStartTime = now.Date.AddHours(7),
                PlanEndTime = now.Date.AddHours(19),
                ActualStartTime = now.Date.AddHours(7),
                Priority = 1,
                CreatedAt = now.AddDays(-1)
            },
            new WorkOrder
            {
                Id = 5,
                WorkOrderNo = "WO-2024-005",
                ProductCode = "PRD-004",
                ProductName = "충전기 케이스",
                LineCode = "LINE-01",
                LineName = "1라인",
                PlanQuantity = 600,
                ProducedQuantity = 0,
                DefectQuantity = 0,
                Status = WorkOrderStatus.Planned,
                PlanStartTime = now.Date.AddDays(1).AddHours(8),
                PlanEndTime = now.Date.AddDays(1).AddHours(17),
                Priority = 4,
                CreatedAt = now.AddHours(-1)
            }
        };

        // 필터링 적용
        var filtered = workOrders.AsEnumerable();

        if (fromDate.HasValue)
        {
            filtered = filtered.Where(w => w.PlanStartTime >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            filtered = filtered.Where(w => w.PlanStartTime <= toDate.Value.Date.AddDays(1).AddTicks(-1));
        }

        if (status.HasValue)
        {
            filtered = filtered.Where(w => w.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(lineCode))
        {
            filtered = filtered.Where(w => w.LineCode == lineCode);
        }

        return filtered;
    }

    public async Task<WorkOrder?> GetWorkOrderAsync(string workOrderNo)
    {
        try
        {
            var response = await _apiClient.GetAsync<ApiResponse<WorkOrder>>($"/api/workorders/{workOrderNo}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 조회 실패: {WorkOrderNo}", workOrderNo);
            return null;
        }
    }

    public async Task<bool> StartWorkOrderAsync(string workOrderNo, string equipmentCode)
    {
        try
        {
            var request = new { EquipmentCode = equipmentCode };
            var response = await _apiClient.PostAsync<object, ApiResponse<bool>>($"/api/workorders/{workOrderNo}/start", request);
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 시작 실패: {WorkOrderNo}", workOrderNo);
            return false;
        }
    }

    public async Task<bool> PauseWorkOrderAsync(string workOrderNo, string reason)
    {
        try
        {
            var request = new { Reason = reason };
            var response = await _apiClient.PostAsync<object, ApiResponse<bool>>($"/api/workorders/{workOrderNo}/pause", request);
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 일시정지 실패: {WorkOrderNo}", workOrderNo);
            return false;
        }
    }

    public async Task<bool> ResumeWorkOrderAsync(string workOrderNo)
    {
        try
        {
            var response = await _apiClient.PostAsync<object, ApiResponse<bool>>($"/api/workorders/{workOrderNo}/resume", new { });
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 재개 실패: {WorkOrderNo}", workOrderNo);
            return false;
        }
    }

    public async Task<bool> CompleteWorkOrderAsync(string workOrderNo)
    {
        try
        {
            var response = await _apiClient.PostAsync<object, ApiResponse<bool>>($"/api/workorders/{workOrderNo}/complete", new { });
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 완료 실패: {WorkOrderNo}", workOrderNo);
            return false;
        }
    }

    public async Task<IEnumerable<WorkOrder>> GetTodayWorkOrdersAsync(string? lineCode = null)
    {
        var today = DateTime.Today;
        return await GetWorkOrdersAsync(today, today, null, lineCode);
    }

    public async Task<IEnumerable<WorkOrder>> GetActiveWorkOrdersAsync()
    {
        return await GetWorkOrdersAsync(status: WorkOrderStatus.InProgress);
    }
}
