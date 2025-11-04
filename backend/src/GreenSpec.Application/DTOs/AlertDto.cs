namespace GreenSpec.Application.DTOs;

public record AlertDto(
    int Id,
    string Type,
    decimal Value,
    decimal Threshold,
    DateTime CreatedAt,
    string Status
);
