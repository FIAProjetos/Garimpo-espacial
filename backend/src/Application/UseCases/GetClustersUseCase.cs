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
}
