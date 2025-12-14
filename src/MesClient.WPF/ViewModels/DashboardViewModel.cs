using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 대시보드 ViewModel
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly IWorkOrderService _workOrderService;
    private readonly IEquipmentService _equipmentService;
    private readonly IProductionService _productionService;
    private readonly IAlarmService _alarmService;

    // KPI 데이터
    [ObservableProperty]
    private int _todayPlanQuantity;

    [ObservableProperty]
    private int _todayProducedQuantity;

    [ObservableProperty]
    private double _todayAchievementRate;

    [ObservableProperty]
    private double _todayDefectRate;

    [ObservableProperty]
    private int _activeWorkOrderCount;

    [ObservableProperty]
    private int _runningEquipmentCount;

    [ObservableProperty]
    private int _totalEquipmentCount;

    [ObservableProperty]
    private double _overallEquipmentEfficiency;

    // 목록 데이터
    [ObservableProperty]
    private ObservableCollection<WorkOrder> _activeWorkOrders = new();

    [ObservableProperty]
    private ObservableCollection<Equipment> _equipments = new();

    [ObservableProperty]
    private ObservableCollection<Alarm> _activeAlarms = new();

    [ObservableProperty]
    private ObservableCollection<Production> _recentProductions = new();

    // 차트 데이터
    [ObservableProperty]
    private ObservableCollection<HourlyProduction> _hourlyProductions = new();

    public DashboardViewModel(
        IWorkOrderService workOrderService,
        IEquipmentService equipmentService,
        IProductionService productionService,
        IAlarmService alarmService)
    {
        _workOrderService = workOrderService;
        _equipmentService = equipmentService;
        _productionService = productionService;
        _alarmService = alarmService;

        Title = "대시보드";
    }

    public override async Task InitializeAsync()
    {
        try
        {
            await RefreshDataAsync();
        }
        catch (Exception ex)
        {
            // 예외 발생 시에도 앱이 크래시되지 않도록 처리
            System.Diagnostics.Debug.WriteLine($"Dashboard 초기화 오류: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // 병렬로 데이터 로드
            var workOrdersTask = _workOrderService.GetActiveWorkOrdersAsync();
            var equipmentsTask = _equipmentService.GetEquipmentsAsync();
            var summaryTask = _productionService.GetTodaySummaryAsync();
            var alarmsTask = _alarmService.GetActiveAlarmsAsync();

            await Task.WhenAll(workOrdersTask, equipmentsTask, summaryTask, alarmsTask);

            // 작업지시
            var workOrders = await workOrdersTask;
            ActiveWorkOrders = new ObservableCollection<WorkOrder>(workOrders);
            ActiveWorkOrderCount = ActiveWorkOrders.Count;

            // 설비
            var equipments = await equipmentsTask;
            Equipments = new ObservableCollection<Equipment>(equipments);
            TotalEquipmentCount = Equipments.Count;
            RunningEquipmentCount = Equipments.Count(e => e.Status == Core.Enums.EquipmentStatus.Running);

            // 생산 요약
            var summary = await summaryTask;
            TodayPlanQuantity = summary.TotalPlanQuantity;
            TodayProducedQuantity = summary.TotalProducedQuantity;
            TodayAchievementRate = summary.AchievementRate;
            TodayDefectRate = summary.DefectRate;

            // 알람
            var alarms = await alarmsTask;
            ActiveAlarms = new ObservableCollection<Alarm>(alarms);
        });
    }

    [RelayCommand]
    private void ViewWorkOrderDetails(WorkOrder workOrder)
    {
        // TODO: 작업지시 상세 화면으로 이동
    }

    [RelayCommand]
    private void ViewEquipmentDetails(Equipment equipment)
    {
        // TODO: 설비 상세 화면으로 이동
    }

    [RelayCommand]
    private async Task AcknowledgeAlarmAsync(Alarm alarm)
    {
        await _alarmService.AcknowledgeAlarmAsync(alarm.Id);
        await RefreshDataAsync();
    }
}

/// <summary>
/// 시간대별 생산 데이터
/// </summary>
public class HourlyProduction
{
    public int Hour { get; set; }
    public string HourLabel => $"{Hour:D2}:00";
    public int PlanQuantity { get; set; }
    public int ProducedQuantity { get; set; }
}
