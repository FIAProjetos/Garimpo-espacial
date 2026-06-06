using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;

namespace Garimpo.Application.UseCases;

/// <summary>
/// Caso de uso de leitura: lista os aglomerados gerados, ordenados por densidade.
/// </summary>
public sealed class GetClustersUseCase
{
    private readonly IClusterRepository _clusterRepository;

    public GetClustersUseCase(IClusterRepository clusterRepository)
    {
        _clusterRepository = clusterRepository;
    }

    public async Task<IReadOnlyList<ClusterDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var clusters = await _clusterRepository.GetAllAsync(cancellationToken);

        return clusters
            .OrderByDescending(c => c.Density)
            .Select(ClusterDto.FromEntity)
            .ToList();
    }

    public async Task<PagedResultDto<ClusterDto>> ListPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var totalCount = await _clusterRepository.CountAsync(cancellationToken);
        var clusters = await _clusterRepository.GetPagedAsync(page, pageSize, cancellationToken);
        var items = clusters.Select(ClusterDto.FromEntity).ToList();

        return PagedResultDto<ClusterDto>.Create(items, page, pageSize, totalCount);
    }
}
