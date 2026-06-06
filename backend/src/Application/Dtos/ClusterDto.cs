using Garimpo.Domain.Entities;

namespace Garimpo.Application.Dtos;

/// <summary>
/// Representacao de leitura de um aglomerado de detritos (zona de alta densidade).
/// </summary>
public sealed record ClusterDto(
    Guid Id,
    int Label,
    double CentroidAltitudeKm,
    double CentroidInclinationDegrees,
    int MemberCount,
    double Density,
    DateTime CreatedAt)
{
    public static ClusterDto FromEntity(Cluster cluster) => new(
        cluster.Id,
        cluster.Label,
        cluster.CentroidAltitudeKm,
        cluster.CentroidInclinationDegrees,
        cluster.MemberCount,
        cluster.Density,
        cluster.CreatedAt);
}
