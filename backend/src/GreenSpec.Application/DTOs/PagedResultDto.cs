namespace GreenSpec.Application.DTOs;

public record PagedResultDto<T>(
    IEnumerable<T> Data,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
)
{
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
