using MesClient.Core.Enums;
using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 설비 서비스 인터페이스
/// </summary>
public interface IEquipmentService
{
    /// <summary>
    /// 설비 목록 조회
    /// </summary>
    Task<IEnumerable<Equipment>> GetEquipmentsAsync(string? lineCode = null);

    /// <summary>
    /// 설비 상세 조회
    /// </summary>
    Task<Equipment?> GetEquipmentAsync(string equipmentCode);

    /// <summary>
    /// 설비 상태 조회
    /// </summary>
    Task<EquipmentStatus> GetEquipmentStatusAsync(string equipmentCode);

    /// <summary>
    /// 설비 상태 변경
    /// </summary>
    Task<bool> UpdateEquipmentStatusAsync(string equipmentCode, EquipmentStatus status, string? reason = null);

    /// <summary>
    /// 설비 가동률 조회
    /// </summary>
    Task<double> GetUtilizationRateAsync(string equipmentCode, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// 라인별 설비 현황
    /// </summary>
    Task<IDictionary<string, IEnumerable<Equipment>>> GetEquipmentsByLineAsync();

    /// <summary>
    /// 설비 상태 변경 이벤트
    /// </summary>
    event EventHandler<EquipmentStatusChangedEventArgs>? StatusChanged;
}

/// <summary>
/// 설비 상태 변경 이벤트 인자
/// </summary>
public class EquipmentStatusChangedEventArgs : EventArgs
{
    public string EquipmentCode { get; set; } = string.Empty;
    public EquipmentStatus PreviousStatus { get; set; }
    public EquipmentStatus CurrentStatus { get; set; }
    public DateTime ChangedAt { get; set; }
}
