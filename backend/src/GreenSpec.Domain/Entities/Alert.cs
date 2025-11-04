using GreenSpec.Domain.Enums;

namespace GreenSpec.Domain.Entities;

public class Alert : BaseEntity
{
    public SensorType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal Threshold { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public AlertStatus Status { get; private set; }

    private Alert() { } // EF Core

    private Alert(SensorType type, decimal value, decimal threshold)
    {
        Type = type;
        Value = value;
        Threshold = threshold;
        CreatedAt = DateTime.UtcNow;
        Status = AlertStatus.Open;
    }

    public static Alert Create(SensorType type, decimal value, decimal threshold)
    {
        return new Alert(type, value, threshold);
    }

    public void Acknowledge()
    {
        if (Status == AlertStatus.Acknowledged)
            throw new InvalidOperationException("Alert is already acknowledged");

        Status = AlertStatus.Acknowledged;
    }

    public bool IsOpen() => Status == AlertStatus.Open;
}
