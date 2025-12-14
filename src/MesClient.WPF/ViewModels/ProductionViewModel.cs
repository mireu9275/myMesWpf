using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 생산관리 ViewModel
/// </summary>
public partial class ProductionViewModel : ViewModelBase
{
    private readonly IWorkOrderService _workOrderService;
    private readonly IProductionService _productionService;
    private readonly IEquipmentService _equipmentService;
    private readonly IDialogService _dialogService;

    // 검색 조건
    [ObservableProperty]
    private DateTime _fromDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _toDate = DateTime.Today;

    [ObservableProperty]
    private WorkOrderStatus? _selectedStatus;

    [ObservableProperty]
    private string? _selectedLineCode;

    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    // 목록
    [ObservableProperty]
    private ObservableCollection<WorkOrder> _workOrders = new();

    [ObservableProperty]
    private WorkOrder? _selectedWorkOrder;

    [ObservableProperty]
    private ObservableCollection<Production> _productions = new();

    [ObservableProperty]
    private ObservableCollection<string> _lineCodes = new();

    // 요약
    [ObservableProperty]
    private DailyProductionSummary? _summary;

    public ProductionViewModel(
        IWorkOrderService workOrderService,
        IProductionService productionService,
        IEquipmentService equipmentService,
        IDialogService dialogService)
    {
        _workOrderService = workOrderService;
        _productionService = productionService;
        _equipmentService = equipmentService;
        _dialogService = dialogService;

        Title = "생산관리";
    }

    public override async Task InitializeAsync()
    {
        await LoadLineCodesAsync();
        await SearchAsync();
    }

    private async Task LoadLineCodesAsync()
    {
        var equipmentsByLine = await _equipmentService.GetEquipmentsByLineAsync();
        LineCodes = new ObservableCollection<string>(equipmentsByLine.Keys);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await ExecuteAsync(async () =>
        {
            var workOrders = await _workOrderService.GetWorkOrdersAsync(
                FromDate, ToDate, SelectedStatus, SelectedLineCode);

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                workOrders = workOrders.Where(w =>
                    w.WorkOrderNo.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    w.ProductName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            WorkOrders = new ObservableCollection<WorkOrder>(workOrders);

            Summary = await _productionService.GetTodaySummaryAsync(SelectedLineCode);
        });
    }

    [RelayCommand]
    private async Task LoadProductionsAsync()
    {
        if (SelectedWorkOrder == null) return;

        await ExecuteAsync(async () =>
        {
            var productions = await _productionService.GetProductionsAsync(
                FromDate, ToDate, SelectedWorkOrder.WorkOrderNo);
            Productions = new ObservableCollection<Production>(productions);
        });
    }

    [RelayCommand]
    private async Task StartWorkOrderAsync()
    {
        if (SelectedWorkOrder == null) return;

        if (SelectedWorkOrder.Status != WorkOrderStatus.Planned &&
            SelectedWorkOrder.Status != WorkOrderStatus.Waiting)
        {
            await _dialogService.ShowWarningAsync("시작할 수 없는 상태입니다.");
            return;
        }

        var confirm = await _dialogService.ShowConfirmAsync(
            $"작업지시 '{SelectedWorkOrder.WorkOrderNo}'를 시작하시겠습니까?");

        if (confirm)
        {
            // TODO: 설비 선택 다이얼로그
            var success = await _workOrderService.StartWorkOrderAsync(
                SelectedWorkOrder.WorkOrderNo, 
                SelectedWorkOrder.EquipmentCode ?? "");

            if (success)
            {
                await _dialogService.ShowInfoAsync("작업지시가 시작되었습니다.");
                await SearchAsync();
            }
            else
            {
                await _dialogService.ShowErrorAsync("작업지시 시작에 실패했습니다.");
            }
        }
    }

    [RelayCommand]
    private async Task PauseWorkOrderAsync()
    {
        if (SelectedWorkOrder == null) return;

        var reason = await _dialogService.ShowInputAsync("일시정지 사유를 입력하세요:", "작업지시 일시정지");
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            var success = await _workOrderService.PauseWorkOrderAsync(
                SelectedWorkOrder.WorkOrderNo, reason);

            if (success)
            {
                await SearchAsync();
            }
        }
    }

    [RelayCommand]
    private async Task CompleteWorkOrderAsync()
    {
        if (SelectedWorkOrder == null) return;

        var confirm = await _dialogService.ShowConfirmAsync(
            $"작업지시 '{SelectedWorkOrder.WorkOrderNo}'를 완료하시겠습니까?");

        if (confirm)
        {
            var success = await _workOrderService.CompleteWorkOrderAsync(
                SelectedWorkOrder.WorkOrderNo);

            if (success)
            {
                await _dialogService.ShowInfoAsync("작업지시가 완료되었습니다.");
                await SearchAsync();
            }
        }
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FromDate = DateTime.Today;
        ToDate = DateTime.Today;
        SelectedStatus = null;
        SelectedLineCode = null;
        SearchKeyword = string.Empty;
    }

    partial void OnSelectedWorkOrderChanged(WorkOrder? value)
    {
        if (value != null)
        {
            _ = LoadProductionsAsync();
        }
    }
}
