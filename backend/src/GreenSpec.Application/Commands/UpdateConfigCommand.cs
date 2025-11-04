using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Commands;

public record UpdateConfigCommand(
    decimal TempMax,
    decimal HumidityMax
) : IRequest<ConfigDto>;
