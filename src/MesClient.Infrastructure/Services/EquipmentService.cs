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
            
            if (response?.Success == true && response.Data != null)
            {
                return response.Data;
            }
            
            // API 응답이 없거나 실패한 경우 예시 데이터 반환
            return GetMockEquipments(lineCode);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "설비 목록 조회 실패");
            // 예시 데이터 반환
            try
            {
                return GetMockEquipments(lineCode);
            }
            catch (Exception mockEx)
            {
                _logger.Error(mockEx, "예시 데이터 생성 실패");
                return Enumerable.Empty<Equipment>();
            }
        }
    }

    private IEnumerable<Equipment> GetMockEquipments(string? lineCode = null)
    {
        var equipments = new List<Equipment>
        {
            new Equipment
            {
                Id = 1,
                EquipmentCode = "EQ-001",
                EquipmentName = "사출기 1호",
                LineCode = "LINE-01",
                LineName = "1라인",
                ProcessCode = "PROC-001",
                ProcessName = "사출공정",
                Status = EquipmentStatus.Running,
                CurrentWorkOrderNo = "WO-2024-001",
                UtilizationRate = 85.5,
                OperatorId = "OP001",
                OperatorName = "김철수",
                LastMaintenanceDate = DateTime.Now.AddDays(-15),
                NextMaintenanceDate = DateTime.Now.AddDays(15),
                CreatedAt = DateTime.Now.AddMonths(-6)
            },
            new Equipment
            {
                Id = 2,
                EquipmentCode = "EQ-002",
                EquipmentName = "사출기 2호",
                LineCode = "LINE-01",
                LineName = "1라인",
                ProcessCode = "PROC-001",
                ProcessName = "사출공정",
                Status = EquipmentStatus.Idle,
                UtilizationRate = 72.3,
                OperatorId = "OP002",
                OperatorName = "이영희",
                LastMaintenanceDate = DateTime.Now.AddDays(-10),
                NextMaintenanceDate = DateTime.Now.AddDays(20),
                CreatedAt = DateTime.Now.AddMonths(-6)
            },
            new Equipment
            {
                Id = 3,
                EquipmentCode = "EQ-003",
                EquipmentName = "조립기 1호",
                LineCode = "LINE-02",
                LineName = "2라인",
                ProcessCode = "PROC-002",
                ProcessName = "조립공정",
                Status = EquipmentStatus.Running,
                CurrentWorkOrderNo = "WO-2024-004",
                UtilizationRate = 91.2,
                OperatorId = "OP003",
                OperatorName = "박민수",
                LastMaintenanceDate = DateTime.Now.AddDays(-5),
                NextMaintenanceDate = DateTime.Now.AddDays(25),
                CreatedAt = DateTime.Now.AddMonths(-4)
            },
            new Equipment
            {
                Id = 4,
                EquipmentCode = "EQ-004",
                EquipmentName = "조립기 2호",
                LineCode = "LINE-02",
                LineName = "2라인",
                ProcessCode = "PROC-002",
                ProcessName = "조립공정",
                Status = EquipmentStatus.Maintenance,
                UtilizationRate = 0,
                LastMaintenanceDate = DateTime.Now.AddDays(-1),
                NextMaintenanceDate = DateTime.Now.AddDays(1),
                CreatedAt = DateTime.Now.AddMonths(-4)
            },
            new Equipment
            {
                Id = 5,
                EquipmentCode = "EQ-005",
                EquipmentName = "포장기 1호",
                LineCode = "LINE-03",
                LineName = "3라인",
                ProcessCode = "PROC-003",
                ProcessName = "포장공정",
                Status = EquipmentStatus.Running,
                CurrentWorkOrderNo = "WO-2024-006",
                UtilizationRate = 78.9,
                OperatorId = "OP004",
                OperatorName = "정수진",
                LastMaintenanceDate = DateTime.Now.AddDays(-20),
                NextMaintenanceDate = DateTime.Now.AddDays(10),
                CreatedAt = DateTime.Now.AddMonths(-3)
            },
            new Equipment
            {
                Id = 6,
                EquipmentCode = "EQ-006",
                EquipmentName = "검사기 1호",
                LineCode = "LINE-03",
                LineName = "3라인",
                ProcessCode = "PROC-004",
                ProcessName = "검사공정",
                Status = EquipmentStatus.Down,
                UtilizationRate = 0,
                LastMaintenanceDate = DateTime.Now.AddDays(-30),
                NextMaintenanceDate = DateTime.Now.AddDays(1),
                CreatedAt = DateTime.Now.AddMonths(-5)
            }
        };

        if (!string.IsNullOrEmpty(lineCode))
        {
            return equipments.Where(e => e.LineCode == lineCode);
        }

        return equipments;
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
                .Where(e => !string.IsNullOrWhiteSpace(e.LineCode))
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
