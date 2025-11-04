using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.ValueObjects;

namespace GreenSpec.Domain.Services;

public class AlertGenerationService
{
    public List<Alert> EvaluateReading(SensorReading reading, Config config)
    {
        var alerts = new List<Alert>();

        // Check temperature threshold
        var tempThreshold = new Threshold(SensorType.Temperature, config.TempMax);
        if (tempThreshold.IsExceeded(reading.Temperature))
        {
            alerts.Add(Alert.Create(
                SensorType.Temperature,
                reading.Temperature,
                config.TempMax
            ));
        }

        // Check humidity threshold
        var humidityThreshold = new Threshold(SensorType.Humidity, config.HumidityMax);
        if (humidityThreshold.IsExceeded(reading.Humidity))
        {
            alerts.Add(Alert.Create(
                SensorType.Humidity,
                reading.Humidity,
                config.HumidityMax
            ));
        }

        return alerts;
    }
}
