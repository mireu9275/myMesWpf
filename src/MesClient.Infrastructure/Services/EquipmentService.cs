using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;
using Serilog;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 설비 서비스 구현
/// </summary>
public class EquipmentService : IEquipmentService
{
    private readonly IApiClient _apiClient;
    private readonly ILogger _logger;

    public event EventHandler<EquipmentStatusChangedEventArgs>? StatusChanged;

    public EquipmentService(IApiClient apiClient, ILogger logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentsAsync(string? lineCode = null)
    {
        try
        {
            var query = !string.IsNullOrEmpty(lineCode) ? $"?lineCode={lineCode}" : "";
            var response = await _apiClient.GetAsync<ApiResponse<IEnumerable<Equipment>>>($"/api/equipments{query}");
            return response?.Data ?? Enumerable.Empty<Equipment>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "설비 목록 조회 실패");
            return Enumerable.Empty<Equipment>();
        }
    }

    public async Task<Equipment?> GetEquipmentAsync(string equipmentCode)
    {
        try
        {
            var response = await _apiClient.GetAsync<ApiResponse<Equipment>>($"/api/equipments/{equipmentCode}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "설비 조회 실패: {EquipmentCode}", equipmentCode);
            return null;
        }
    }

    public async Task<EquipmentStatus> GetEquipmentStatusAsync(string equipmentCode)
    {
        var equipment = await GetEquipmentAsync(equipmentCode);
        return equipment?.Status ?? EquipmentStatus.Offline;
    }

    public async Task<bool> UpdateEquipmentStatusAsync(string equipmentCode, EquipmentStatus status, string? reason = null)
    {
        try
        {
            var request = new { Status = (int)status, Reason = reason };
            var response = await _apiClient.PutAsync<object, ApiResponse<bool>>($"/api/equipments/{equipmentCode}/status", request);
            
            if (response?.Success == true)
            {
                StatusChanged?.Invoke(this, new EquipmentStatusChangedEventArgs
                {
                    EquipmentCode = equipmentCode,
                    CurrentStatus = status,
                    ChangedAt = DateTime.Now
                });
            }
            
            return response?.Success == true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "설비 상태 변경 실패: {EquipmentCode}", equipmentCode);
            return false;
        }
    }

    public async Task<double> GetUtilizationRateAsync(string equipmentCode, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var query = $"?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var response = await _apiClient.GetAsync<ApiResponse<double>>($"/api/equipments/{equipmentCode}/utilization{query}");
            return response?.Data ?? 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "설비 가동률 조회 실패: {EquipmentCode}", equipmentCode);
            return 0;
        }
    }

    public async Task<IDictionary<string, IEnumerable<Equipment>>> GetEquipmentsByLineAsync()
    {
        try
        {
            var equipments = await GetEquipmentsAsync();
            return equipments
                .GroupBy(e => e.LineCode)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "라인별 설비 조회 실패");
            return new Dictionary<string, IEnumerable<Equipment>>();
        }
    }
}
