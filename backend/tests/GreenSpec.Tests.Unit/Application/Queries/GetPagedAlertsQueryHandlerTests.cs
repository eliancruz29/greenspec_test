using FluentAssertions;
using GreenSpec.Application.Queries;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using Moq;

namespace GreenSpec.Tests.Unit.Application.Queries;

public class GetPagedAlertsQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepositoryMock;
    private readonly GetPagedAlertsQueryHandler _handler;

    public GetPagedAlertsQueryHandlerTests()
    {
        _alertRepositoryMock = new Mock<IAlertRepository>();
        _handler = new GetPagedAlertsQueryHandler(_alertRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithoutFilters_ShouldReturnPagedAlerts()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m),
            Alert.Create(SensorType.Humidity, 90m, 80m)
        };

        var query = new GetPagedAlertsQuery(null, null, null, 1, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((alerts, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
        var dataList = result.Data.ToList();
        dataList[0].Type.Should().Be("Temperature");
        dataList[1].Type.Should().Be("Humidity");
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ShouldPassFilterToRepository()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m)
        };

        var query = new GetPagedAlertsQuery("Open", null, null, 1, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(AlertStatus.Open, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((alerts, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        _alertRepositoryMock.Verify(
            x => x.GetPagedAsync(AlertStatus.Open, null, null, 1, 10, It.IsAny<CancellationToken>()),
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

        var query = new GetPagedAlertsQuery(null, from, to, 1, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, from, to, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((alerts, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _alertRepositoryMock.Verify(
            x => x.GetPagedAsync(null, from, to, 1, 10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapAlertsToDto()
    {
        // Arrange
        var alert = Alert.Create(SensorType.Temperature, 35m, 30m);
        var query = new GetPagedAlertsQuery(null, null, null, 1, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Alert> { alert }, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        var dto = result.Data.First();
        dto.Type.Should().Be("Temperature");
        dto.Value.Should().Be(35m);
        dto.Threshold.Should().Be(30m);
        dto.Status.Should().Be("Open");
    }

    [Fact]
    public async Task Handle_WithNoResults_ShouldReturnEmptyPagedResult()
    {
        // Arrange
        var query = new GetPagedAlertsQuery(null, null, null, 1, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Alert>(), 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m),
            Alert.Create(SensorType.Humidity, 90m, 80m)
        };

        var query = new GetPagedAlertsQuery(null, null, null, 1, 5);

        // Simulate 25 total alerts, returning 5 per page
        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, null, null, 1, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((alerts, 25));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(25);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(5); // 25 / 5 = 5 pages
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_OnLastPage_ShouldIndicateNoNextPage()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            Alert.Create(SensorType.Temperature, 35m, 30m)
        };

        var query = new GetPagedAlertsQuery(null, null, null, 3, 10);

        _alertRepositoryMock
            .Setup(x => x.GetPagedAsync(null, null, null, 3, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((alerts, 25));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(3);
        result.TotalPages.Should().Be(3); // 25 / 10 = 3 pages (ceiling)
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeFalse();
    }
}
