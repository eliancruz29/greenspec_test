using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Enums;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GreenSpec.Infrastructure.Repositories;

public class AlertRepository(ApplicationDbContext context) : IAlertRepository
{
    public async Task<Alert?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Alerts.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetAllAsync(
        AlertStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Alerts.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= toDate.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        context.Alerts.Add(alert);
        await context.SaveChangesAsync(cancellationToken);
        return alert;
    }

    public async Task<Alert> UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        context.Alerts.Update(alert);
        await context.SaveChangesAsync(cancellationToken);
        return alert;
    }
}
