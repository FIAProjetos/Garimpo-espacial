using Garimpo.Domain.Entities;

namespace Garimpo.Application.Ports;

/// <summary>
/// Porta de saida (driven port) para persistencia de aglomerados.
/// </summary>
public interface IClusterRepository
{
    Task AddRangeAsync(IEnumerable<Cluster> clusters, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cluster>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cluster>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<Cluster?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Remove todos os aglomerados existentes (antes de uma nova execucao de clustering).</summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
