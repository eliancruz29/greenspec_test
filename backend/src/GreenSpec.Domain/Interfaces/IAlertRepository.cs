using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;

namespace GreenSpec.Domain.Interfaces;

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetAllAsync(
        AlertStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken = default);
    Task<Alert> UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
}
