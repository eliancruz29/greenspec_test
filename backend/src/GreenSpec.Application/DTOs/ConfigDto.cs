namespace GreenSpec.Application.DTOs;

public record ConfigDto(
    int Id,
    decimal TempMax,
    decimal HumidityMax,
    DateTime UpdatedAt
);
