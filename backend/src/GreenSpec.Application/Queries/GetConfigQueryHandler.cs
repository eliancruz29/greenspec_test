using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Queries;

public class GetConfigQueryHandler(IConfigRepository configRepository)
    : IRequestHandler<GetConfigQuery, ConfigDto>
{
    public async Task<ConfigDto> Handle(GetConfigQuery request, CancellationToken cancellationToken)
    {
        var config = await configRepository.GetCurrentConfigAsync(cancellationToken)
            ?? throw new InvalidOperationException("Configuration not found");

        return new ConfigDto(
            config.Id,
            config.TempMax,
            config.HumidityMax,
            config.UpdatedAt
        );
    }
}
