using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.SpaceAssets;

/// <summary>
/// Satelite ativo em operacao. Herda de <see cref="OrbitalAsset"/> e representa
/// infraestrutura critica que precisa ser protegida contra colisoes com detritos.
/// </summary>
public sealed class Satellite : OrbitalAsset
{
    public bool IsOperational { get; private set; }
    public string Operator { get; private set; } = string.Empty;

    private Satellite()
    {
    }

    public static Satellite Create(
        int noradId,
        string name,
        double altitudeKm,
        double inclinationDegrees,
        string operatorName,
        bool isOperational = true)
    {
        return new Satellite
        {
            Id = Guid.NewGuid(),
            NoradId = noradId,
            Name = name,
            AltitudeKm = altitudeKm,
            InclinationDegrees = inclinationDegrees,
            Operator = operatorName,
            IsOperational = isOperational,
            RegisteredAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Satelites operacionais em LEO tem risco elevado de colisao com detritos.
    /// </summary>
    public override double CalculateRiskScore()
    {
        if (!IsOperational)
        {
            return 10;
        }

        double altitudeRisk = AltitudeKm switch
        {
            < 600 => 90,
            < 1_000 => 70,
            < 2_000 => 50,
            _ => 20
        };

        return Math.Min(100, altitudeRisk);
    }

    public override string GetSummary()
        => $"Satelite {Name} ({Operator}) em {AltitudeKm:F0} km - risco {CalculateRiskScore():F0}%";
}
