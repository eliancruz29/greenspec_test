using FluentAssertions;
using GreenSpec.Domain.ValueObjects;

namespace GreenSpec.Tests.Unit.Domain.ValueObjects;

public class SensorReadingTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSensorReading()
    {
        // Arrange
        var temperature = 25m;
        var humidity = 60m;

        // Act
        var reading = new SensorReading(temperature, humidity);

        // Assert
        reading.Should().NotBeNull();
        reading.Temperature.Should().Be(temperature);
        reading.Humidity.Should().Be(humidity);
        reading.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(-50, 60)]
    [InlineData(0, 60)]
    [InlineData(50, 60)]
    [InlineData(100, 60)]
    public void Constructor_WithValidTemperatureRange_ShouldCreateSensorReading(decimal temperature, decimal humidity)
    {
        // Act
        var reading = new SensorReading(temperature, humidity);

        // Assert
        reading.Temperature.Should().Be(temperature);
    }

    [Theory]
    [InlineData(-51)]
    [InlineData(-100)]
    [InlineData(101)]
    [InlineData(150)]
    public void Constructor_WithInvalidTemperature_ShouldThrowArgumentOutOfRangeException(decimal temperature)
    {
        // Arrange
        var humidity = 60m;

        // Act
        var act = () => new SensorReading(temperature, humidity);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("temperature");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Constructor_WithValidHumidityRange_ShouldCreateSensorReading(decimal humidity)
    {
        // Arrange
        var temperature = 25m;

        // Act
        var reading = new SensorReading(temperature, humidity);

        // Assert
        reading.Humidity.Should().Be(humidity);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(101)]
    [InlineData(150)]
    public void Constructor_WithInvalidHumidity_ShouldThrowArgumentOutOfRangeException(decimal humidity)
    {
        // Arrange
        var temperature = 25m;

        // Act
        var act = () => new SensorReading(temperature, humidity);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("humidity");
    }

    [Fact]
    public void Equals_WithDifferentTimestamps_ShouldNotBeEqual()
    {
        // Arrange
        var reading1 = new SensorReading(25m, 60m);
        Thread.Sleep(10);
        var reading2 = new SensorReading(25m, 60m);

        // Act & Assert
        // Records compare all properties including Timestamp, so these should not be equal
        reading1.Should().NotBe(reading2);
        reading1.Temperature.Should().Be(reading2.Temperature);
        reading1.Humidity.Should().Be(reading2.Humidity);
        reading1.Timestamp.Should().BeBefore(reading2.Timestamp);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var reading1 = new SensorReading(25m, 60m);
        var reading2 = new SensorReading(30m, 70m);

        // Act & Assert
        reading1.Should().NotBe(reading2);
    }
}
