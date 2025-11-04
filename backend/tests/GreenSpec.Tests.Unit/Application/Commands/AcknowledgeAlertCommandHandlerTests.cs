using FluentAssertions;
using GreenSpec.Application.Commands;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using Moq;

namespace GreenSpec.Tests.Unit.Application.Commands;

public class AcknowledgeAlertCommandHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock;
    private readonly AcknowledgeAlertCommandHandler _handler;

    public AcknowledgeAlertCommandHandlerTests()
    {
        _alertRepositoryMock = new Mock<IAlertRepository>();
        _handler = new AcknowledgeAlertCommandHandler(_alertRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAcknowledgeAlert()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Temperature, 35m, 30m);
        var command = new AcknowledgeAlertCommand(1);

        _alertRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        _alertRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alert a, CancellationToken _) => a);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Acknowledged");

        _alertRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);

        _alertRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAlertNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new AcknowledgeAlertCommand(999);

        _alertRepositoryMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alert?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Alert with ID 999 not found");
    }

    [Fact]
    public async Task Handle_ShouldUpdateAlertStatus()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Humidity, 90m, 80m);
        var command = new AcknowledgeAlertCommand(1);

        _alertRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alert);

        _alertRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alert a, CancellationToken _) => a);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _alertRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Alert>(a => a.Status == AlertStatus.Acknowledged),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
