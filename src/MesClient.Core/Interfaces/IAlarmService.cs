using MesClient.Core.Models;

namespace MesClient.Core.Interfaces;

/// <summary>
/// 알람 서비스 인터페이스
/// </summary>
public interface IAlarmService
{
    /// <summary>
    /// 활성 알람 목록
    /// </summary>
    Task<IEnumerable<Alarm>> GetActiveAlarmsAsync();

    /// <summary>
    /// 알람 이력 조회
    /// </summary>
    Task<IEnumerable<Alarm>> GetAlarmHistoryAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string? equipmentCode = null,
        AlarmLevel? level = null);

    /// <summary>
    /// 알람 확인 처리
    /// </summary>
    Task<bool> AcknowledgeAlarmAsync(long alarmId);

    /// <summary>
    /// 알람 해제 처리
    /// </summary>
    Task<bool> ClearAlarmAsync(long alarmId, string? action = null);

    /// <summary>
    /// 실시간 알람 구독
    /// </summary>
    IObservable<Alarm> SubscribeToAlarms();

    /// <summary>
    /// 알람 발생 이벤트
    /// </summary>
    event EventHandler<Alarm>? AlarmOccurred;

    /// <summary>
    /// 알람 해제 이벤트
    /// </summary>
    event EventHandler<Alarm>? AlarmCleared;
}
