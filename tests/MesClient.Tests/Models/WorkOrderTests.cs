using FluentAssertions;
using MesClient.Core.Models;
using Xunit;

namespace MesClient.Tests.Models;

public class WorkOrderTests
{
    [Theory]
    [InlineData(100, 50, 50.0)]
    [InlineData(100, 0, 0.0)]
    [InlineData(100, 100, 100.0)]
    [InlineData(0, 0, 0.0)]
    public void ProgressRate_ShouldCalculateCorrectly(int planQty, int producedQty, double expected)
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            PlanQuantity = planQty,
            ProducedQuantity = producedQty
        };

        // Act
        var result = workOrder.ProgressRate;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(100, 5, 5.0)]
    [InlineData(100, 0, 0.0)]
    [InlineData(0, 0, 0.0)]
    public void DefectRate_ShouldCalculateCorrectly(int producedQty, int defectQty, double expected)
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            ProducedQuantity = producedQty,
            DefectQuantity = defectQty
        };

        // Act
        var result = workOrder.DefectRate;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GoodQuantity_ShouldBeProducedMinusDefect()
    {
        // Arrange
        var workOrder = new WorkOrder
        {
            ProducedQuantity = 100,
            DefectQuantity = 5
        };

        // Act
        var result = workOrder.GoodQuantity;

        // Assert
        result.Should().Be(95);
    }
}
