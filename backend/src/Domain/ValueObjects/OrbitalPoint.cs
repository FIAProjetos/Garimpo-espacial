namespace Garimpo.Domain.ValueObjects;

/// <summary>
/// Ponto no espaco de caracteristicas (feature space) usado pelo DBSCAN.
/// Em vez de propagar a posicao 3D instantanea (SGP4), agrupamos os detritos pelo
/// "regime orbital" - altitude e inclinacao - que define onde o objeto vive ao longo
/// do tempo. Isso e suficiente e mais robusto para mapear zonas de densidade de lixo.
/// </summary>
public readonly record struct OrbitalPoint
{
    /// <summary>Identificador do detrito ao qual este ponto pertence.</summary>
    public Guid DebrisId { get; }

    /// <summary>Altitude em km.</summary>
    public double AltitudeKm { get; }

    /// <summary>Inclinacao em graus.</summary>
    public double InclinationDegrees { get; }

    public OrbitalPoint(Guid debrisId, double altitudeKm, double inclinationDegrees)
    {
        DebrisId = debrisId;
        AltitudeKm = altitudeKm;
        InclinationDegrees = inclinationDegrees;
    }
}
