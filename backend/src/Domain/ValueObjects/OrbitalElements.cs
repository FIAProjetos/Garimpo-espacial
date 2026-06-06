using Garimpo.Domain.Enums;

namespace Garimpo.Domain.ValueObjects;

/// <summary>
/// Value Object imutavel que representa os elementos orbitais keplerianos extraidos de um TLE
/// (Two-Line Element). Encapsula a fisica necessaria para classificar e agrupar detritos.
/// </summary>
public readonly record struct OrbitalElements
{
    /// <summary>Constante gravitacional padrao da Terra (mu) em km^3/s^2.</summary>
    private const double EarthGravitationalParameter = 398_600.4418;

    /// <summary>Raio equatorial medio da Terra em km.</summary>
    private const double EarthRadiusKm = 6_378.137;

    /// <summary>Segundos em um dia solar medio.</summary>
    private const double SecondsPerDay = 86_400.0;

    /// <summary>Inclinacao orbital em graus.</summary>
    public double InclinationDegrees { get; }

    /// <summary>Excentricidade da orbita (0 = circular).</summary>
    public double Eccentricity { get; }

    /// <summary>Movimento medio em revolucoes por dia.</summary>
    public double MeanMotionRevsPerDay { get; }

    /// <summary>Ascensao reta do nodo ascendente (RAAN) em graus.</summary>
    public double RightAscensionDegrees { get; }

    public OrbitalElements(
        double inclinationDegrees,
        double eccentricity,
        double meanMotionRevsPerDay,
        double rightAscensionDegrees)
    {
        if (meanMotionRevsPerDay <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(meanMotionRevsPerDay),
                "O movimento medio deve ser maior que zero para derivar a altitude orbital.");
        }

        InclinationDegrees = inclinationDegrees;
        Eccentricity = eccentricity;
        MeanMotionRevsPerDay = meanMotionRevsPerDay;
        RightAscensionDegrees = rightAscensionDegrees;
    }

    /// <summary>
    /// Semi-eixo maior em km, derivado do movimento medio via terceira lei de Kepler.
    /// </summary>
    public double SemiMajorAxisKm
    {
        get
        {
            double meanMotionRadPerSec = MeanMotionRevsPerDay * 2.0 * Math.PI / SecondsPerDay;
            return Math.Cbrt(EarthGravitationalParameter / (meanMotionRadPerSec * meanMotionRadPerSec));
        }
    }

    /// <summary>
    /// Altitude media aproximada acima da superficie terrestre, em km.
    /// </summary>
    public double AltitudeKm => SemiMajorAxisKm - EarthRadiusKm;

    /// <summary>
    /// Classifica o detrito conforme a faixa de altitude derivada.
    /// </summary>
    public DebrisClassification Classify()
    {
        double altitude = AltitudeKm;

        return altitude switch
        {
            < 2_000 => DebrisClassification.LowEarthOrbit,
            < 34_000 => DebrisClassification.MediumEarthOrbit,
            < 37_500 => DebrisClassification.GeostationaryOrbit,
            _ => DebrisClassification.HighEarthOrbit
        };
    }
}
