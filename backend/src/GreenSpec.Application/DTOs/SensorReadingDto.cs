namespace GreenSpec.Application.DTOs;

public record SensorReadingDto(
    decimal Temperature,
    decimal Humidity,
    DateTime Timestamp
);
