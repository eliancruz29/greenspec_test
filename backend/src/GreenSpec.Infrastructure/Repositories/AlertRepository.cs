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

    public async Task<(IEnumerable<Alert> Alerts, int TotalCount)> GetPagedAsync(
        AlertStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
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

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var alerts = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (alerts, totalCount);
    }

    public async Task<IEnumerable<Alert>> AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default)
    {
        context.Alerts.AddRange(alerts);
        await context.SaveChangesAsync(cancellationToken);
        return alerts;
    }

    public async Task<Alert> UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        context.Alerts.Update(alert);
        await context.SaveChangesAsync(cancellationToken);
        return alert;
    }
}
