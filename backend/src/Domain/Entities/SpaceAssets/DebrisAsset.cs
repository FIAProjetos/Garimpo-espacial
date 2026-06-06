using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.SpaceAssets;

/// <summary>
/// Representacao de dominio de um detrito orbital (polimorfismo sobre <see cref="OrbitalAsset"/>).
/// Encapsula a entidade persistida <see cref="Debris"/> sem acoplar o dominio ao EF Core.
/// </summary>
public sealed class DebrisAsset : OrbitalAsset
{
    public Enums.DebrisClassification DomainClassification { get; private set; }

    private DebrisAsset()
    {
    }

    public static DebrisAsset FromEntity(Debris debris)
    {
        return new DebrisAsset
        {
            Id = debris.Id,
            NoradId = debris.NoradId,
            Name = debris.Name,
            AltitudeKm = debris.AltitudeKm,
            InclinationDegrees = debris.InclinationDegrees,
            DomainClassification = debris.Classification,
            RegisteredAt = debris.CapturedAt
        };
    }

    /// <summary>Detritos em LEO proximo a satelites ativos representam risco maximo.</summary>
    public override double CalculateRiskScore()
    {
        return DomainClassification switch
        {
            Enums.DebrisClassification.LowEarthOrbit => Math.Min(100, 40 + AltitudeKm / 20),
            Enums.DebrisClassification.MediumEarthOrbit => 30,
            _ => 10
        };
    }

    public override string GetSummary()
        => $"Detrito {Name} (NORAD {NoradId}) em {AltitudeKm:F0} km - risco {CalculateRiskScore():F0}%";
}
