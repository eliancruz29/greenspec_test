namespace GreenSpec.Domain.ValueObjects;

public record SensorReading
{
    public decimal Temperature { get; init; }
    public decimal Humidity { get; init; }
    public DateTime Timestamp { get; init; }

    public SensorReading(decimal temperature, decimal humidity)
    {
        if (temperature < -50 || temperature > 100)
            throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between -50 and 100");

        if (humidity < 0 || humidity > 100)
            throw new ArgumentOutOfRangeException(nameof(humidity), "Humidity must be between 0 and 100");

        Temperature = temperature;
        Humidity = humidity;
        Timestamp = DateTime.UtcNow;
    }
}
