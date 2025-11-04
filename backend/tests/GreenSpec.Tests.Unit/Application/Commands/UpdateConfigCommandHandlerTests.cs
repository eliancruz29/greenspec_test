using FluentAssertions;
using GreenSpec.Application.Commands;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using Moq;

namespace GreenSpec.Tests.Unit.Application.Commands;

public class UpdateConfigCommandHandlerTests
{
    private readonly Mock<IConfigRepository> _configRepositoryMock;
    private readonly UpdateConfigCommandHandler _handler;

    public UpdateConfigCommandHandlerTests()
    {
        _configRepositoryMock = new Mock<IConfigRepository>();
        _handler = new UpdateConfigCommandHandler(_configRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateConfig()
    {
        // Arrange
        var existingConfig = Config.Create(30m, 80m);
        var command = new UpdateConfigCommand(35m, 75m);

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);

        _configRepositoryMock
            .Setup(x => x.UpdateConfigAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config c, CancellationToken _) => c);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TempMax.Should().Be(35m);
        result.HumidityMax.Should().Be(75m);

        _configRepositoryMock.Verify(
            x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _configRepositoryMock.Verify(
            x => x.UpdateConfigAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenConfigNotFound_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new UpdateConfigCommand(35m, 75m);

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Configuration not found");
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectParametersToRepository()
    {
        // Arrange
        var existingConfig = Config.Create(30m, 80m);
        var command = new UpdateConfigCommand(40m, 85m);

        _configRepositoryMock
            .Setup(x => x.GetCurrentConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConfig);

        _configRepositoryMock
            .Setup(x => x.UpdateConfigAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config c, CancellationToken _) => c);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _configRepositoryMock.Verify(
            x => x.UpdateConfigAsync(
                It.Is<Config>(c => c.TempMax == 40m && c.HumidityMax == 85m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
