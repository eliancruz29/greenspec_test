namespace GreenSpec.Application.DTOs;

public record LoginResponse(
    string Token,
    string Username,
    DateTime ExpiresAt
);
