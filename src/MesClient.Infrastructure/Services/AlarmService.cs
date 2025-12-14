using System.Reactive.Linq;
using System.Reactive.Subjects;
using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.Infrastructure.Api;

namespace MesClient.Infrastructure.Services;

/// <summary>
/// 알람 서비스 구현
/// </summary>
public class AlarmService : IAlarmService
{
    private readonly IApiClient _apiClient;
    private readonly Subject<Alarm> _alarmSubject = new();

    public event EventHandler<Alarm>? AlarmOccurred;
    public event EventHandler<Alarm>? AlarmCleared;

    public AlarmService(IApiClient apiClient)
    {
        _apiClient = apiClient;
        
        // TODO: SignalR 또는 WebSocket 연동을 통해 실시간 알람 수신 구현 필요
        // 현재는 주기적 폴링이나 모의 데이터로 대체 가능
    }

    public async Task<IEnumerable<Alarm>> GetActiveAlarmsAsync()
    {
        try
        {
            return await _apiClient.GetAsync<IEnumerable<Alarm>>("/api/alarms/active");
        }
        catch
        {
            // API 호출 실패 시 빈 목록 반환 (데모용)
            return Enumerable.Empty<Alarm>();
        }
    }

    public async Task<IEnumerable<Alarm>> GetAlarmHistoryAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string? equipmentCode = null,
        AlarmLevel? level = null)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["fromDate"] = fromDate.ToString("yyyy-MM-dd"),
            ["toDate"] = toDate.ToString("yyyy-MM-dd")
        };

        if (!string.IsNullOrEmpty(equipmentCode))
            queryParams["equipmentCode"] = equipmentCode;

        if (level.HasValue)
            queryParams["level"] = level.Value.ToString();

        try
        {
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}"));
            return await _apiClient.GetAsync<IEnumerable<Alarm>>("/api/alarms/history?" + queryString);
        }
        catch
        {
            return Enumerable.Empty<Alarm>();
        }
    }

    public async Task<bool> AcknowledgeAlarmAsync(long alarmId)
    {
        try
        {
            await _apiClient.PostAsync<object, object>($"/api/alarms/{alarmId}/acknowledge", null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ClearAlarmAsync(long alarmId, string? action = null)
    {
        try
        {
            var data = new { Action = action };
            await _apiClient.PostAsync<object, object>($"/api/alarms/{alarmId}/clear", data);
            
            // 로컬 이벤트 발생 (데모)
            var alarm = new Alarm { Id = alarmId, ClearedAt = DateTime.Now };
            AlarmCleared?.Invoke(this, alarm);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public IObservable<Alarm> SubscribeToAlarms()
    {
        return _alarmSubject.AsObservable();
    }
}
