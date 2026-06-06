using Garimpo.Domain.ValueObjects;

namespace Garimpo.Application.Ports;

/// <summary>
/// Porta que abstrai o algoritmo de clusterizacao espacial (DBSCAN) para os casos de uso,
/// mantendo a camada de aplicacao independente da implementacao concreta do dominio.
/// </summary>
public interface IClusteringService
{
    /// <summary>
    /// Agrupa os pontos orbitais e retorna o mapa de DebrisId para rotulo de cluster (-1 = ruido).
    /// </summary>
    IReadOnlyDictionary<Guid, int> Cluster(
        IReadOnlyList<OrbitalPoint> points,
        double epsilon,
        int minPoints);
}
