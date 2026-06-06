using Garimpo.Application.Ports;
using Garimpo.Domain.Services;
using Garimpo.Domain.ValueObjects;

namespace Garimpo.Infrastructure.Adapters;

/// <summary>
/// Liga a porta <see cref="IClusteringService"/> da aplicacao a implementacao pura
/// do DBSCAN no dominio (<see cref="DbscanClusteringService"/>).
/// </summary>
public sealed class DbscanClusteringAdapter : IClusteringService
{
    private readonly DbscanClusteringService _dbscan = new();

    public IReadOnlyDictionary<Guid, int> Cluster(
        IReadOnlyList<OrbitalPoint> points,
        double epsilon,
        int minPoints)
        => _dbscan.Cluster(points, epsilon, minPoints);
}
