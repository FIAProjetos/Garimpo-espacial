using Garimpo.Domain.Entities;

namespace Garimpo.Application.Ports;

/// <summary>
/// Porta de saida (driven port) para persistencia de detritos.
/// Implementada por um adapter de infraestrutura (EF Core).
/// </summary>
public interface IDebrisRepository
{
    Task AddRangeAsync(IEnumerable<Debris> debris, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Debris>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna todos os detritos rastreados pelo contexto, para que reatribuicoes de
    /// aglomerado sejam persistidas corretamente durante a clusterizacao.
    /// </summary>
    Task<IReadOnlyList<Debris>> GetAllTrackedAsync(CancellationToken cancellationToken = default);

    Task<Debris?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Retorna os NORAD IDs ja existentes no catalogo (para deduplicacao na ingestao).</summary>
    Task<IReadOnlySet<int>> GetExistingNoradIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>Remove o vinculo de todos os detritos com aglomerados (antes de re-clusterizar).</summary>
    Task ClearClusterAssignmentsAsync(CancellationToken cancellationToken = default);
}
