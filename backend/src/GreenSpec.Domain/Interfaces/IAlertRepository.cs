using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;

namespace GreenSpec.Domain.Interfaces;

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Alert> Alerts, int TotalCount)> GetPagedAsync(
        AlertStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);
    Task<Alert> UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
}
