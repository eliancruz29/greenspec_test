using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Commands;

public class AcknowledgeAlertCommandHandler(IAlertRepository alertRepository)
    : IRequestHandler<AcknowledgeAlertCommand, AlertDto>
{
    public async Task<AlertDto> Handle(AcknowledgeAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await alertRepository.GetByIdAsync(request.AlertId, cancellationToken)
            ?? throw new InvalidOperationException($"Alert with ID {request.AlertId} not found");

        alert.Acknowledge();

        var updated = await alertRepository.UpdateAsync(alert, cancellationToken);

        return new AlertDto(
            updated.Id,
            updated.Type.ToString(),
            updated.Value,
            updated.Threshold,
            updated.CreatedAt,
            updated.Status.ToString()
        );
    }
}
