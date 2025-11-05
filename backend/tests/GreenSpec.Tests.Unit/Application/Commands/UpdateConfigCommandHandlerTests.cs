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
    public async Task Handle_WithValidCommand_ShouldCreateNewConfigVersion()
    {
        // Arrange
        var command = new UpdateConfigCommand(35m, 75m);

        _configRepositoryMock
            .Setup(x => x.DeactivateAllConfigsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _configRepositoryMock
            .Setup(x => x.CreateNewConfigVersionAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config c, CancellationToken _) => c);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TempMax.Should().Be(35m);
        result.HumidityMax.Should().Be(75m);

        _configRepositoryMock.Verify(
            x => x.DeactivateAllConfigsAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _configRepositoryMock.Verify(
            x => x.CreateNewConfigVersionAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateAllConfigsBeforeCreatingNew()
    {
        // Arrange
        var command = new UpdateConfigCommand(35m, 75m);
        var deactivateCalled = false;
        var createCalled = false;

        _configRepositoryMock
            .Setup(x => x.DeactivateAllConfigsAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                deactivateCalled = true;
                createCalled.Should().BeFalse("Deactivate should be called before Create");
            })
            .Returns(Task.CompletedTask);

        _configRepositoryMock
            .Setup(x => x.CreateNewConfigVersionAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                createCalled = true;
                deactivateCalled.Should().BeTrue("Create should be called after Deactivate");
            })
            .ReturnsAsync((Config c, CancellationToken _) => c);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        deactivateCalled.Should().BeTrue();
        createCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCreateNewConfigWithCorrectParameters()
    {
        // Arrange
        var command = new UpdateConfigCommand(40m, 85m);

        _configRepositoryMock
            .Setup(x => x.DeactivateAllConfigsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _configRepositoryMock
            .Setup(x => x.CreateNewConfigVersionAsync(It.IsAny<Config>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Config c, CancellationToken _) => c);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _configRepositoryMock.Verify(
            x => x.CreateNewConfigVersionAsync(
                It.Is<Config>(c => c.TempMax == 40m && c.HumidityMax == 85m && c.IsActive == true),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
