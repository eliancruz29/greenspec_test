using GreenSpec.Application.DTOs;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using MediatR;

namespace GreenSpec.Application.Queries;

public class GetPagedAlertsQueryHandler(IAlertRepository alertRepository)
    : IRequestHandler<GetPagedAlertsQuery, PagedResultDto<AlertDto>>
{
    public async Task<PagedResultDto<AlertDto>> Handle(GetPagedAlertsQuery request, CancellationToken cancellationToken)
    {
        AlertStatus? status = request.Status?.ToLower() switch
        {
            "open" => AlertStatus.Open,
            "acknowledged" or "ack" => AlertStatus.Acknowledged,
            _ => null
        };

        var (alerts, totalCount) = await alertRepository.GetPagedAsync(
            status,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        var alertDtos = alerts.Select(a => new AlertDto(
            a.Id,
            a.Type.ToString(),
            a.Value,
            a.Threshold,
            a.CreatedAt,
            a.Status.ToString()
        ));

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResultDto<AlertDto>(
            alertDtos,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );
    }
}
