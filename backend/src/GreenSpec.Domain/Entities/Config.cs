namespace GreenSpec.Domain.Entities;

public class Config : BaseEntity
{
    public decimal TempMax { get; private set; }
    public decimal HumidityMax { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Config() { } // EF Core

    private Config(decimal tempMax, decimal humidityMax)
    {
        TempMax = tempMax;
        HumidityMax = humidityMax;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Config Create(decimal tempMax, decimal humidityMax)
    {
        if (tempMax <= 0)
            throw new ArgumentException("Temperature threshold must be greater than 0", nameof(tempMax));

        if (humidityMax <= 0 || humidityMax > 100)
            throw new ArgumentException("Humidity threshold must be between 0 and 100", nameof(humidityMax));

        return new Config(tempMax, humidityMax);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateThresholds(decimal tempMax, decimal humidityMax)
    {
        if (tempMax <= 0)
            throw new ArgumentException("Temperature threshold must be greater than 0", nameof(tempMax));

        if (humidityMax <= 0 || humidityMax > 100)
            throw new ArgumentException("Humidity threshold must be between 0 and 100", nameof(humidityMax));

        TempMax = tempMax;
        HumidityMax = humidityMax;
        UpdatedAt = DateTime.UtcNow;
    }
}
