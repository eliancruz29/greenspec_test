using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Commands;

public class UpdateConfigCommandHandler(IConfigRepository configRepository)
    : IRequestHandler<UpdateConfigCommand, ConfigDto>
{
    public async Task<ConfigDto> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
    {
        var config = await configRepository.GetCurrentConfigAsync(cancellationToken)
            ?? throw new InvalidOperationException("Configuration not found");

        config.UpdateThresholds(request.TempMax, request.HumidityMax);

        var updated = await configRepository.UpdateConfigAsync(config, cancellationToken);

        return new ConfigDto(
            updated.Id,
            updated.TempMax,
            updated.HumidityMax,
            updated.UpdatedAt
        );
    }
}
