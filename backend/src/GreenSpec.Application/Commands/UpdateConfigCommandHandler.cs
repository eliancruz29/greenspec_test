using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Commands;

public class UpdateConfigCommandHandler(IConfigRepository configRepository)
    : IRequestHandler<UpdateConfigCommand, ConfigDto>
{
    public async Task<ConfigDto> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
    {
        // Deactivate all existing configurations
        await configRepository.DeactivateAllConfigsAsync(cancellationToken);

        // Create new configuration version
        var newConfig = Config.Create(request.TempMax, request.HumidityMax);
        var created = await configRepository.CreateNewConfigVersionAsync(newConfig, cancellationToken);

        return new ConfigDto(
            created.Id,
            created.TempMax,
            created.HumidityMax,
            created.UpdatedAt
        );
    }
}
