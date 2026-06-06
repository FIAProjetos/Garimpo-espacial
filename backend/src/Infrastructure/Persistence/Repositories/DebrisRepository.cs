using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garimpo.Infrastructure.Persistence.Repositories;

/// <summary>Adapter EF Core para <see cref="IDebrisRepository"/>.</summary>
public sealed class DebrisRepository : IDebrisRepository
{
    private readonly GarimpoDbContext _context;

    public DebrisRepository(GarimpoDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Debris> debris, CancellationToken cancellationToken = default)
    {
        await _context.Debris.AddRangeAsync(debris, cancellationToken);
    }

    public async Task<IReadOnlyList<Debris>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Debris
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Debris>> GetAllTrackedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Debris.ToListAsync(cancellationToken);
    }

    public async Task<Debris?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Debris
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Debris.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlySet<int>> GetExistingNoradIdsAsync(CancellationToken cancellationToken = default)
    {
        var ids = await _context.Debris
            .AsNoTracking()
            .Select(d => d.NoradId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task ClearClusterAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        await _context.Debris
            .Where(d => d.ClusterId != null)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.ClusterId, (Guid?)null), cancellationToken);
    }
}
