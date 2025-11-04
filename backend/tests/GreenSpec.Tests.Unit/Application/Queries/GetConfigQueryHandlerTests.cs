using FluentAssertions;
using GreenSpec.Application.Queries;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using Moq;

namespace GreenSpec.Tests.Unit.Application.Queries;

public class GetConfigQueryHandlerTests
{
    private readonly Mock<IConfigRepository> _configRepositoryMock;
    private readonly GetConfigQueryHandler _handler;

    public GetConfigQueryHandlerTests()
    {
        _configRepositoryMock = new Mock<IConfigRepository>();
        _handler = new GetConfigQueryHandler(_configRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenConfigExists_ShouldReturnConfigDto()
    {
        // Arrange
        var config = Config.Create(30m, 80m);
        var query = new GetConfigQuery();

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TempMax.Should().Be(30m);
        result.HumidityMax.Should().Be(80m);

        _configRepositoryMock.Verify(
            x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var query = new GetConfigQuery();

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Configuration not found");
    }

    [Fact]
    public async Task Handle_ShouldMapConfigToDto()
    {
        // Arrange
        var config = Config.Create(35m, 75m);
        var query = new GetConfigQuery();

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TempMax.Should().Be(config.TempMax);
        result.HumidityMax.Should().Be(config.HumidityMax);
        result.UpdatedAt.Should().Be(config.UpdatedAt);
    }
}
