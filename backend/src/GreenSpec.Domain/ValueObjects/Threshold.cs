using GreenSpec.Domain.Enums;

namespace GreenSpec.Domain.ValueObjects;

public record Threshold
{
    public SensorType Type { get; init; }
    public decimal Value { get; init; }

    public Threshold(SensorType type, decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("Threshold value must be greater than 0", nameof(value));

        Type = type;
        Value = value;
    }

    public bool IsExceeded(decimal currentValue) => currentValue > Value;
}
