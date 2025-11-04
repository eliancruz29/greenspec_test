using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Queries;

public record GetAlertsQuery(
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<IEnumerable<AlertDto>>;
