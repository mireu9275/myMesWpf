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
            var result = await _apiClient.GetAsync<IEnumerable<Alarm>>("/api/alarms/active");
            if (result != null)
            {
                return result;
            }
            
            // API 응답이 없는 경우 예시 데이터 반환
            return GetMockActiveAlarms();
        }
        catch (Exception ex)
        {
            // 예시 데이터 반환
            try
            {
                return GetMockActiveAlarms();
            }
            catch
            {
                return Enumerable.Empty<Alarm>();
            }
        }
    }

    private IEnumerable<Alarm> GetMockActiveAlarms()
    {
        var now = DateTime.Now;
        return new List<Alarm>
        {
            new Alarm
            {
                Id = 1,
                AlarmCode = "ALM-001",
                AlarmName = "온도 이상",
                EquipmentCode = "EQ-001",
                EquipmentName = "사출기 1호",
                Level = AlarmLevel.Warning,
                OccurredAt = now.AddMinutes(-30),
                Description = "설비 온도가 설정값을 초과했습니다.",
                CreatedAt = now.AddMinutes(-30)
            },
            new Alarm
            {
                Id = 2,
                AlarmCode = "ALM-002",
                AlarmName = "압력 부족",
                EquipmentCode = "EQ-003",
                EquipmentName = "조립기 1호",
                Level = AlarmLevel.Error,
                OccurredAt = now.AddMinutes(-15),
                Description = "작동 압력이 기준치 이하로 떨어졌습니다.",
                CreatedAt = now.AddMinutes(-15)
            },
            new Alarm
            {
                Id = 3,
                AlarmCode = "ALM-003",
                AlarmName = "재료 부족",
                EquipmentCode = "EQ-005",
                EquipmentName = "포장기 1호",
                Level = AlarmLevel.Info,
                OccurredAt = now.AddMinutes(-5),
                Description = "포장재가 부족합니다.",
                CreatedAt = now.AddMinutes(-5)
            }
        };
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
            var result = await _apiClient.GetAsync<IEnumerable<Alarm>>("/api/alarms/history?" + queryString);
            if (result != null)
            {
                return result;
            }
            
            // API 응답이 없는 경우 예시 데이터 반환
            return GetMockAlarmHistory(fromDate, toDate, equipmentCode, level);
        }
        catch (Exception ex)
        {
            // 예시 데이터 반환
            try
            {
                return GetMockAlarmHistory(fromDate, toDate, equipmentCode, level);
            }
            catch
            {
                return Enumerable.Empty<Alarm>();
            }
        }
    }

    private IEnumerable<Alarm> GetMockAlarmHistory(
        DateTime fromDate, 
        DateTime toDate, 
        string? equipmentCode = null,
        AlarmLevel? level = null)
    {
        var alarms = new List<Alarm>
        {
            new Alarm
            {
                Id = 10,
                AlarmCode = "ALM-010",
                AlarmName = "온도 이상",
                EquipmentCode = "EQ-001",
                EquipmentName = "사출기 1호",
                Level = AlarmLevel.Warning,
                OccurredAt = fromDate.AddHours(2),
                AcknowledgedAt = fromDate.AddHours(2).AddMinutes(5),
                AcknowledgedBy = "김철수",
                ClearedAt = fromDate.AddHours(2).AddMinutes(10),
                ClearedBy = "김철수",
                Description = "설비 온도가 설정값을 초과했습니다.",
                CreatedAt = fromDate.AddHours(2)
            },
            new Alarm
            {
                Id = 11,
                AlarmCode = "ALM-011",
                AlarmName = "압력 부족",
                EquipmentCode = "EQ-003",
                EquipmentName = "조립기 1호",
                Level = AlarmLevel.Error,
                OccurredAt = fromDate.AddHours(5),
                AcknowledgedAt = fromDate.AddHours(5).AddMinutes(3),
                AcknowledgedBy = "박민수",
                ClearedAt = fromDate.AddHours(5).AddMinutes(15),
                ClearedBy = "박민수",
                Description = "작동 압력이 기준치 이하로 떨어졌습니다.",
                CreatedAt = fromDate.AddHours(5)
            }
        };

        if (!string.IsNullOrEmpty(equipmentCode))
        {
            alarms = alarms.Where(a => a.EquipmentCode == equipmentCode).ToList();
        }

        if (level.HasValue)
        {
            alarms = alarms.Where(a => a.Level == level.Value).ToList();
        }

        return alarms;
    }

    public async Task<bool> AcknowledgeAlarmAsync(long alarmId)
    {
        try
        {
            await _apiClient.PostAsync<object, object>($"/api/alarms/{alarmId}/acknowledge", new { });
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
