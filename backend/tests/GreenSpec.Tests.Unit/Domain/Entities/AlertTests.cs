using FluentAssertions;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;

namespace GreenSpec.Tests.Unit.Domain.Entities;

public class AlertTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAlert()
    {
        // Arrange
        var type = SensorType.Temperature;
        var value = 35m;
        var threshold = 30m;

        // Act
        var alert = Alert.Create(type, value, threshold);

        // Assert
        alert.Should().NotBeNull();
        alert.Type.Should().Be(type);
        alert.Value.Should().Be(value);
        alert.Threshold.Should().Be(threshold);
        alert.Status.Should().Be(AlertStatus.Open);
        alert.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(SensorType.Temperature, 35, 30)]
    [InlineData(SensorType.Humidity, 90, 80)]
    public void Create_WithDifferentSensorTypes_ShouldCreateAlert(SensorType type, decimal value, decimal threshold)
    {
        // Act
        var alert = Alert.Create(type, value, threshold);

        // Assert
        alert.Type.Should().Be(type);
        alert.Value.Should().Be(value);
        alert.Threshold.Should().Be(threshold);
    }

    [Fact]
    public void Acknowledge_WhenAlertIsOpen_ShouldChangeStatusToAcknowledged()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Temperature, 35m, 30m);

        // Act
        alert.Acknowledge();

        // Assert
        alert.Status.Should().Be(AlertStatus.Acknowledged);
    }

    [Fact]
    public void Acknowledge_WhenAlertIsAlreadyAcknowledged_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Temperature, 35m, 30m);
        alert.Acknowledge();

        // Act
        var act = () => alert.Acknowledge();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Alert is already acknowledged");
    }

    [Fact]
    public void Create_MultipleAlerts_ShouldHaveUniqueCreatedAtTimes()
    {
        // Arrange & Act
        var alert1 = Alert.Create(SensorType.Temperature, 35m, 30m);
        Thread.Sleep(10);
        var alert2 = Alert.Create(SensorType.Humidity, 90m, 80m);

        // Assert
        alert1.CreatedAt.Should().BeBefore(alert2.CreatedAt);
    }
}
