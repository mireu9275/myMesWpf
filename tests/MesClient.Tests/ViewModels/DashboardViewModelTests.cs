using FluentAssertions;
using Moq;
using MesClient.Core.Interfaces;
using MesClient.Core.Models;
using MesClient.WPF.ViewModels;
using Xunit;

namespace MesClient.Tests.ViewModels;

public class DashboardViewModelTests
{
    private readonly Mock<IWorkOrderService> _workOrderServiceMock;
    private readonly Mock<IEquipmentService> _equipmentServiceMock;
    private readonly Mock<IProductionService> _productionServiceMock;
    private readonly Mock<IAlarmService> _alarmServiceMock;
    private readonly DashboardViewModel _viewModel;

    public DashboardViewModelTests()
    {
        _workOrderServiceMock = new Mock<IWorkOrderService>();
        _equipmentServiceMock = new Mock<IEquipmentService>();
        _productionServiceMock = new Mock<IProductionService>();
        _alarmServiceMock = new Mock<IAlarmService>();

        _viewModel = new DashboardViewModel(
            _workOrderServiceMock.Object,
            _equipmentServiceMock.Object,
            _productionServiceMock.Object,
            _alarmServiceMock.Object);
    }

    [Fact]
    public void Title_ShouldBe_Dashboard()
    {
        // Assert
        _viewModel.Title.Should().Be("대시보드");
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadData()
    {
        // Arrange
        var workOrders = new List<WorkOrder>
        {
            new() { WorkOrderNo = "WO001", ProductName = "Product A" },
            new() { WorkOrderNo = "WO002", ProductName = "Product B" }
        };

        var equipments = new List<Equipment>
        {
            new() { EquipmentCode = "EQ001", Status = Core.Enums.EquipmentStatus.Running },
            new() { EquipmentCode = "EQ002", Status = Core.Enums.EquipmentStatus.Idle }
        };

        var summary = new DailyProductionSummary
        {
            TotalPlanQuantity = 1000,
            TotalProducedQuantity = 800,
            TotalDefectQuantity = 20
        };

        var alarms = new List<Alarm>();

        _workOrderServiceMock
            .Setup(x => x.GetActiveWorkOrdersAsync())
            .ReturnsAsync(workOrders);

        _equipmentServiceMock
            .Setup(x => x.GetEquipmentsAsync(null))
            .ReturnsAsync(equipments);

        _productionServiceMock
            .Setup(x => x.GetTodaySummaryAsync(null))
            .ReturnsAsync(summary);

        _alarmServiceMock
            .Setup(x => x.GetActiveAlarmsAsync())
            .ReturnsAsync(alarms);

        // Act
        await _viewModel.InitializeAsync();

        // Assert
        _viewModel.ActiveWorkOrders.Should().HaveCount(2);
        _viewModel.Equipments.Should().HaveCount(2);
        _viewModel.TodayPlanQuantity.Should().Be(1000);
        _viewModel.TodayProducedQuantity.Should().Be(800);
        _viewModel.TodayAchievementRate.Should().Be(80.0);
        _viewModel.RunningEquipmentCount.Should().Be(1);
    }

    [Fact]
    public async Task RefreshDataCommand_ShouldReloadData()
    {
        // Arrange
        _workOrderServiceMock
            .Setup(x => x.GetActiveWorkOrdersAsync())
            .ReturnsAsync(new List<WorkOrder>());

        _equipmentServiceMock
            .Setup(x => x.GetEquipmentsAsync(null))
            .ReturnsAsync(new List<Equipment>());

        _productionServiceMock
            .Setup(x => x.GetTodaySummaryAsync(null))
            .ReturnsAsync(new DailyProductionSummary());

        _alarmServiceMock
            .Setup(x => x.GetActiveAlarmsAsync())
            .ReturnsAsync(new List<Alarm>());

        // Act
        await _viewModel.RefreshDataCommand.ExecuteAsync(null);

        // Assert
        _workOrderServiceMock.Verify(x => x.GetActiveWorkOrdersAsync(), Times.Once);
        _equipmentServiceMock.Verify(x => x.GetEquipmentsAsync(null), Times.Once);
        _productionServiceMock.Verify(x => x.GetTodaySummaryAsync(null), Times.Once);
        _alarmServiceMock.Verify(x => x.GetActiveAlarmsAsync(), Times.Once);
    }
}
