namespace Garimpo.Domain.Structs;

/// <summary>
/// Struct auxiliar que representa uma coordenada orbital simplificada (altitude x inclinacao).
/// Usada para calculos de proximidade e exposicao segura em DTOs, sem revelar dados sensiveis
/// completos do TLE.
/// </summary>
public readonly partial struct OrbitalCoordinate
{
    public double AltitudeKm { get; }
    public double InclinationDegrees { get; }

    public OrbitalCoordinate(double altitudeKm, double inclinationDegrees)
    {
        AltitudeKm = altitudeKm;
        InclinationDegrees = inclinationDegrees;
    }

    /// <summary>Distancia euclidiana no espaco de caracteristicas orbital.</summary>
    public double DistanceTo(OrbitalCoordinate other)
    {
        double dAlt = AltitudeKm - other.AltitudeKm;
        double dInc = InclinationDegrees - other.InclinationDegrees;
        return Math.Sqrt(dAlt * dAlt + dInc * dInc);
    }

    public override string ToString()
        => $"Alt {AltitudeKm:F1} km / Inc {InclinationDegrees:F2}°";
}
