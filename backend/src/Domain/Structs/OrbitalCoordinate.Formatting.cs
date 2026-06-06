namespace Garimpo.Domain.Structs;

/// <summary>Parte parcial de <see cref="OrbitalCoordinate"/> para formatacao e validacao.</summary>
public readonly partial struct OrbitalCoordinate
{
    /// <summary>Verifica se a coordenada esta dentro de faixas orbitais plausiveis.</summary>
    public bool IsPlausible()
        => AltitudeKm is >= 100 and <= 100_000
           && InclinationDegrees is >= 0 and <= 180;

    /// <summary>Representacao segura para logs (sem precisao excessiva).</summary>
    public string ToSafeLogString()
        => $"[{AltitudeKm:F0}km, {InclinationDegrees:F1}°]";
}
