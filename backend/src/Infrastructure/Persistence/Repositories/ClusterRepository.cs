using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garimpo.Infrastructure.Persistence.Repositories;

/// <summary>Adapter EF Core para <see cref="IClusterRepository"/>.</summary>
public sealed class ClusterRepository : IClusterRepository
{
    private readonly GarimpoDbContext _context;

    public ClusterRepository(GarimpoDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Cluster> clusters, CancellationToken cancellationToken = default)
    {
        await _context.Clusters.AddRangeAsync(clusters, cancellationToken);
    }

    public async Task<IReadOnlyList<Cluster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clusters
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Cluster>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        int skip = (page - 1) * pageSize;

        return await _context.Clusters
            .AsNoTracking()
            .OrderByDescending(c => c.Density)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clusters.CountAsync(cancellationToken);
    }

    public async Task<Cluster?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Clusters
            .AsNoTracking()
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Desvincula os detritos (FK -> null) antes de remover os aglomerados.
        await _context.Debris
            .Where(d => d.ClusterId != null)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.ClusterId, (Guid?)null), cancellationToken);

        await _context.Clusters.ExecuteDeleteAsync(cancellationToken);
    }
}
