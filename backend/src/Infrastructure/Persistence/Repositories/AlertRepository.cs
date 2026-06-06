using Garimpo.Application.Ports;
using Garimpo.Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;

namespace Garimpo.Infrastructure.Persistence.Repositories;

public sealed class AlertRepository : IAlertRepository
{
    private readonly GarimpoDbContext _context;

    public AlertRepository(GarimpoDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default)
    {
        await _context.Alerts.AddRangeAsync(alerts, cancellationToken);
    }

    public async Task<IReadOnlyList<Alert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .AsNoTracking()
            .OrderByDescending(a => a.TriggeredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Alert?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _context.Alerts.ExecuteDeleteAsync(cancellationToken);
    }
}
