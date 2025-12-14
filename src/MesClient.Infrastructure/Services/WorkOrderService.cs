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
            
            return response?.Data ?? Enumerable.Empty<WorkOrder>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "작업지시 목록 조회 실패");
            return Enumerable.Empty<WorkOrder>();
        }
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
