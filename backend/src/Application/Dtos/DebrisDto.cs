using Garimpo.Domain.Entities;

namespace Garimpo.Application.Dtos;

/// <summary>
/// Representacao de leitura de um detrito orbital exposta pela API.
/// </summary>
public sealed record DebrisDto(
    Guid Id,
    int NoradId,
    string Name,
    double InclinationDegrees,
    double Eccentricity,
    double MeanMotionRevsPerDay,
    double AltitudeKm,
    string Classification,
    DateTime CapturedAt,
    Guid? ClusterId)
{
    public static DebrisDto FromEntity(Debris debris) => new(
        debris.Id,
        debris.NoradId,
        debris.Name,
        debris.InclinationDegrees,
        debris.Eccentricity,
        debris.MeanMotionRevsPerDay,
        debris.AltitudeKm,
        debris.Classification.ToString(),
        debris.CapturedAt,
        debris.ClusterId);
}
