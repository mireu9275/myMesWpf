using MesClient.Core.Enums;
using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 작업지시 서비스 인터페이스
/// </summary>
public interface IWorkOrderService
{
    /// <summary>
    /// 작업지시 목록 조회
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetWorkOrdersAsync(
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        WorkOrderStatus? status = null,
        string? lineCode = null);

    /// <summary>
    /// 작업지시 상세 조회
    /// </summary>
    Task<WorkOrder?> GetWorkOrderAsync(string workOrderNo);

    /// <summary>
    /// 작업지시 시작
    /// </summary>
    Task<bool> StartWorkOrderAsync(string workOrderNo, string equipmentCode);

    /// <summary>
    /// 작업지시 일시정지
    /// </summary>
    Task<bool> PauseWorkOrderAsync(string workOrderNo, string reason);

    /// <summary>
    /// 작업지시 재개
    /// </summary>
    Task<bool> ResumeWorkOrderAsync(string workOrderNo);

    /// <summary>
    /// 작업지시 완료
    /// </summary>
    Task<bool> CompleteWorkOrderAsync(string workOrderNo);

    /// <summary>
    /// 오늘의 작업지시 목록
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetTodayWorkOrdersAsync(string? lineCode = null);

    /// <summary>
    /// 진행중인 작업지시 목록
    /// </summary>
    Task<IEnumerable<WorkOrder>> GetActiveWorkOrdersAsync();
}
