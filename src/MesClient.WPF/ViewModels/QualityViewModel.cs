using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MesClient.Core.Enums;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;

namespace MesClient.WPF.ViewModels;

/// <summary>
/// 품질관리 ViewModel
/// </summary>
public partial class QualityViewModel : ViewModelBase
{
    private readonly IQualityService _qualityService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private DateTime _fromDate = DateTime.Today.AddDays(-7);

    [ObservableProperty]
    private DateTime _toDate = DateTime.Today;

    [ObservableProperty]
    private QualityResult? _selectedResult;

    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<QualityInspection> _inspections = new();

    [ObservableProperty]
    private QualityInspection? _selectedInspection;

    [ObservableProperty]
    private QualitySummary? _summary;

    [ObservableProperty]
    private ObservableCollection<DefectStatistic> _defectStatistics = new();

    public QualityViewModel(
        IQualityService qualityService,
        IDialogService dialogService)
    {
        _qualityService = qualityService;
        _dialogService = dialogService;

        Title = "품질관리";
        
        // Summary 초기화
        Summary = new QualitySummary
        {
            TotalInspections = 0,
            PassCount = 0,
            FailCount = 0
        };
    }

    public override async Task InitializeAsync()
    {
        await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await ExecuteAsync(async () =>
        {
            var inspectionsTask = _qualityService.GetInspectionsAsync(
                FromDate, ToDate, null, SelectedResult);
            var summaryTask = _qualityService.GetQualitySummaryAsync(FromDate, ToDate);
            var defectStatsTask = _qualityService.GetDefectStatisticsAsync(FromDate, ToDate);

            await Task.WhenAll(inspectionsTask, summaryTask, defectStatsTask);

            var inspections = await inspectionsTask;

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                inspections = inspections.Where(i =>
                    i.LotNo.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    i.ProductName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    i.InspectionNo.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            Inspections = new ObservableCollection<QualityInspection>(inspections);
            Summary = await summaryTask;

            var defectStats = await defectStatsTask;
            DefectStatistics = new ObservableCollection<DefectStatistic>(
                defectStats.Select(d => new DefectStatistic 
                { 
                    DefectCode = d.Key, 
                    Count = d.Value 
                }));
        });
    }

    [RelayCommand]
    private async Task NewInspectionAsync()
    {
        // TODO: 품질 검사 입력 다이얼로그
        await _dialogService.ShowInfoAsync("품질 검사 입력 기능은 준비중입니다.");
    }

    [RelayCommand]
    private async Task ViewLotHistoryAsync()
    {
        if (SelectedInspection == null) return;

        var inspections = await _qualityService.GetInspectionsByLotAsync(SelectedInspection.LotNo);
        // TODO: LOT 이력 화면으로 이동
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        // TODO: Excel 내보내기
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FromDate = DateTime.Today.AddDays(-7);
        ToDate = DateTime.Today;
        SelectedResult = null;
        SearchKeyword = string.Empty;
    }
}

/// <summary>
/// 불량 통계
/// </summary>
public class DefectStatistic
{
    public string DefectCode { get; set; } = string.Empty;
    public int Count { get; set; }
}
