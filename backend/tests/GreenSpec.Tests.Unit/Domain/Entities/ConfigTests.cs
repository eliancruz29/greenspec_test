using FluentAssertions;
using GreenSpec.Domain.Entities;

namespace GreenSpec.Tests.Unit.Domain.Entities;

public class ConfigTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateConfig()
    {
        // Arrange
        var tempMax = 30m;
        var humidityMax = 80m;

        // Act
        var config = Config.Create(tempMax, humidityMax);

        // Assert
        config.Should().NotBeNull();
        config.TempMax.Should().Be(tempMax);
        config.HumidityMax.Should().Be(humidityMax);
        config.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidTempMax_ShouldThrowArgumentException(decimal tempMax)
    {
        // Arrange
        var humidityMax = 80m;

        // Act
        var act = () => Config.Create(tempMax, humidityMax);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Temperature threshold must be greater than 0*")
            .And.ParamName.Should().Be("tempMax");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(150)]
    public void Create_WithInvalidHumidityMax_ShouldThrowArgumentException(decimal humidityMax)
    {
        // Arrange
        var tempMax = 30m;

        // Act
        var act = () => Config.Create(tempMax, humidityMax);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Humidity threshold must be between 0 and 100*")
            .And.ParamName.Should().Be("humidityMax");
    }

    [Fact]
    public void UpdateThresholds_WithValidParameters_ShouldUpdateConfig()
    {
        // Arrange
        var config = Config.Create(30m, 80m);
        var originalUpdateTime = config.UpdatedAt;
        var newTempMax = 35m;
        var newHumidityMax = 75m;

        // Wait a small amount to ensure UpdatedAt changes
        Thread.Sleep(10);

        // Act
        config.UpdateThresholds(newTempMax, newHumidityMax);

        // Assert
        config.TempMax.Should().Be(newTempMax);
        config.HumidityMax.Should().Be(newHumidityMax);
        config.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }

    [Theory]
    [InlineData(0, 80)]
    [InlineData(-1, 80)]
    public void UpdateThresholds_WithInvalidTempMax_ShouldThrowArgumentException(decimal tempMax, decimal humidityMax)
    {
        // Arrange
        var config = Config.Create(30m, 80m);

        // Act
        var act = () => config.UpdateThresholds(tempMax, humidityMax);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Temperature threshold must be greater than 0*");
    }

    [Theory]
    [InlineData(30, 0)]
    [InlineData(30, -1)]
    [InlineData(30, 101)]
    public void UpdateThresholds_WithInvalidHumidityMax_ShouldThrowArgumentException(decimal tempMax, decimal humidityMax)
    {
        // Arrange
        var config = Config.Create(30m, 80m);

        // Act
        var act = () => config.UpdateThresholds(tempMax, humidityMax);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Humidity threshold must be between 0 and 100*");
    }
}
