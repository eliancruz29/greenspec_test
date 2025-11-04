using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Queries;

public class GetAlertsQueryHandler(IAlertRepository alertRepository)
    : IRequestHandler<GetAlertsQuery, IEnumerable<AlertDto>>
{
    public async Task<IEnumerable<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        AlertStatus? status = request.Status?.ToLower() switch
        {
            "open" => AlertStatus.Open,
            "acknowledged" or "ack" => AlertStatus.Acknowledged,
            _ => null
        };

        var alerts = await alertRepository.GetAllAsync(
            status,
            request.FromDate,
            request.ToDate,
            cancellationToken
        );

        return alerts.Select(a => new AlertDto(
            a.Id,
            a.Type.ToString(),
            a.Value,
            a.Threshold,
            a.CreatedAt,
            a.Status.ToString()
        ));
    }
}
