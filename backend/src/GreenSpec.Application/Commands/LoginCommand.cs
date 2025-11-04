using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Commands;

public record LoginCommand(
    string Username,
    string Password
) : IRequest<LoginResponse>;
