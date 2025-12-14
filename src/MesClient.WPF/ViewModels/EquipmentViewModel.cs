using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 설비관리 ViewModel
/// </summary>
public partial class EquipmentViewModel : ViewModelBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly IDialogService _dialogService;

    private System.Timers.Timer? _refreshTimer;

    [ObservableProperty]
    private string? _selectedLineCode;

    [ObservableProperty]
    private EquipmentStatus? _selectedStatus;

    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Equipment> _equipments = new();

    [ObservableProperty]
    private Equipment? _selectedEquipment;

    [ObservableProperty]
    private ObservableCollection<string> _lineCodes = new();

    // 상태별 통계
    [ObservableProperty]
    private int _runningCount;

    [ObservableProperty]
    private int _idleCount;

    [ObservableProperty]
    private int _downCount;

    [ObservableProperty]
    private int _maintenanceCount;

    public EquipmentViewModel(
        IEquipmentService equipmentService,
        IDialogService dialogService)
    {
        _equipmentService = equipmentService;
        _dialogService = dialogService;

        Title = "설비관리";

        _equipmentService.StatusChanged += OnEquipmentStatusChanged;
    }

    public override async Task InitializeAsync()
    {
        await LoadDataAsync();
        StartAutoRefresh();
    }

    public override Task CleanupAsync()
    {
        StopAutoRefresh();
        return Task.CompletedTask;
    }

    private void StartAutoRefresh()
    {
        _refreshTimer = new System.Timers.Timer(5000); // 5초마다
        _refreshTimer.Elapsed += async (s, e) => await LoadDataAsync();
        _refreshTimer.Start();
    }

    private void StopAutoRefresh()
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }

    private void OnEquipmentStatusChanged(object? sender, EquipmentStatusChangedEventArgs e)
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var equipmentsByLine = await _equipmentService.GetEquipmentsByLineAsync();
            LineCodes = new ObservableCollection<string>(equipmentsByLine.Keys);

            var allEquipments = equipmentsByLine.Values.SelectMany(e => e);

            // 필터 적용
            var filtered = allEquipments.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedLineCode))
            {
                filtered = filtered.Where(e => e.LineCode == SelectedLineCode);
            }

            if (SelectedStatus.HasValue)
            {
                filtered = filtered.Where(e => e.Status == SelectedStatus.Value);
            }

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                filtered = filtered.Where(e =>
                    e.EquipmentCode.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    e.EquipmentName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            Equipments = new ObservableCollection<Equipment>(filtered);

            // 통계 업데이트
            RunningCount = allEquipments.Count(e => e.Status == EquipmentStatus.Running);
            IdleCount = allEquipments.Count(e => e.Status == EquipmentStatus.Idle);
            DownCount = allEquipments.Count(e => e.Status == EquipmentStatus.Down);
            MaintenanceCount = allEquipments.Count(e => e.Status == EquipmentStatus.Maintenance);
        });
    }

    [RelayCommand]
    private async Task ChangeStatusAsync(EquipmentStatus newStatus)
    {
        if (SelectedEquipment == null) return;

        var statusName = newStatus switch
        {
            EquipmentStatus.Running => "가동",
            EquipmentStatus.Idle => "대기",
            EquipmentStatus.Down => "고장",
            EquipmentStatus.Maintenance => "정비",
            EquipmentStatus.Setup => "셋업",
            _ => newStatus.ToString()
        };

        var confirm = await _dialogService.ShowConfirmAsync(
            $"설비 '{SelectedEquipment.EquipmentName}'의 상태를 '{statusName}'(으)로 변경하시겠습니까?");

        if (confirm)
        {
            string? reason = null;
            if (newStatus == EquipmentStatus.Down || newStatus == EquipmentStatus.Maintenance)
            {
                reason = await _dialogService.ShowInputAsync("사유를 입력하세요:");
            }

            var success = await _equipmentService.UpdateEquipmentStatusAsync(
                SelectedEquipment.EquipmentCode, newStatus, reason);

            if (success)
            {
                await _dialogService.ShowInfoAsync("설비 상태가 변경되었습니다.");
                await LoadDataAsync();
            }
            else
            {
                await _dialogService.ShowErrorAsync("설비 상태 변경에 실패했습니다.");
            }
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SelectedLineCode = null;
        SelectedStatus = null;
        SearchKeyword = string.Empty;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void ViewDetails(Equipment equipment)
    {
        // TODO: 설비 상세 화면으로 이동
    }
}
