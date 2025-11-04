using GreenSpec.Application.DTOs;
using GreenSpec.Application.Interfaces;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Commands;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Username);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new LoginResponse(token, expiresAt);
    }
}
