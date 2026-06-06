using Garimpo.Domain.Enums;
using Garimpo.Domain.Structs;

namespace Garimpo.Domain.Entities.SpaceAssets;

/// <summary>
/// Ativo orbital abstrato: satelite ativo ou detrito compartilham elementos keplerianos
/// e calculo de risco de colisao.
/// </summary>
public abstract class OrbitalAsset : SpaceAsset
{
    public int NoradId { get; protected set; }
    public double AltitudeKm { get; protected set; }
    public double InclinationDegrees { get; protected set; }

    public override AssetCategory Category => AssetCategory.Orbital;

    public OrbitalCoordinate ToCoordinate() => new(AltitudeKm, InclinationDegrees);

    /// <summary>Score de risco polimorfico (0-100). Cada subtipo calcula de forma distinta.</summary>
    public abstract double CalculateRiskScore();

    public override string GetTrackingId() => $"NORAD-{NoradId}";
}
