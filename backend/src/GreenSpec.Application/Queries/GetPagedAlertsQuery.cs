using GreenSpec.Application.DTOs;
using MediatR;

namespace GreenSpec.Application.Queries;

public record GetPagedAlertsQuery(
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResultDto<AlertDto>>;
