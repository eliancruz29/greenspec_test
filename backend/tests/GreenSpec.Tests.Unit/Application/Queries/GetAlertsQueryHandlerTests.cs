using FluentAssertions;
using GreenSpec.Application.Queries;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using Moq;

namespace GreenSpec.Tests.Unit.Application.Queries;

public class GetAlertsQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock;
    private readonly GetAlertsQueryHandler _handler;

    public GetAlertsQueryHandlerTests()
    {
        _alertRepositoryMock = new Mock<IAlertRepository>();
        _handler = new GetAlertsQueryHandler(_alertRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnAllAlerts()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m),
            Alert.Create(SensorType.Humidity, 90m, 80m)
        };

        var query = new GetAlertsQuery(null, null, null);

        _alertRepositoryMock
            .Setup(x => x.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(2);
        resultList[0].Type.Should().Be("Temperature");
        resultList[1].Type.Should().Be("Humidity");
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ShouldPassFilterToRepository()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m)
        };

        var query = new GetAlertsQuery("Open", null, null);

        _alertRepositoryMock
            .Setup(x => x.GetAllAsync(AlertStatus.Open, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ToList().Should().HaveCount(1);
        _alertRepositoryMock.Verify(
            x => x.GetAllAsync(AlertStatus.Open, null, null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithDateRangeFilter_ShouldPassFilterToRepository()
    {
        // Arrange
        var from = DateTime.UtcNow.AddHours(-1);
        var to = DateTime.UtcNow;
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m)
        };

        var query = new GetAlertsQuery(null, from, to);

        _alertRepositoryMock
            .Setup(x => x.GetAllAsync(null, from, to, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _alertRepositoryMock.Verify(
            x => x.GetAllAsync(null, from, to, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAlertsToDto()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Temperature, 35m, 30m);
        var query = new GetAlertsQuery(null, null, null);

        _alertRepositoryMock
            .Setup(x => x.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert> { alert });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(1);
        var dto = resultList[0];
        dto.Type.Should().Be("Temperature");
        dto.Value.Should().Be(35m);
        dto.Threshold.Should().Be(30m);
        dto.Status.Should().Be("Open");
    }

    [Fact]
    public async Task Handle_WithNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAlertsQuery(null, null, null);

        _alertRepositoryMock
            .Setup(x => x.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
