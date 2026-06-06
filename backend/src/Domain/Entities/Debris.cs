using Garimpo.Domain.Enums;
using Garimpo.Domain.ValueObjects;

namespace Garimpo.Domain.Entities;

/// <summary>
/// Entidade atomica do dominio: representa um detrito orbital individual ingerido a partir
/// de um TLE (Two-Line Element) do catalogo Celestrak/NORAD.
/// </summary>
public partial class Debris
{
    /// <summary>Identidade interna da plataforma.</summary>
    public Guid Id { get; private set; }

    /// <summary>Identificador no catalogo NORAD (unico por objeto rastreado).</summary>
    public int NoradId { get; private set; }

    /// <summary>Nome/designacao do objeto.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Primeira linha do TLE bruto (fonte da verdade).</summary>
    public string Line1 { get; private set; } = string.Empty;

    /// <summary>Segunda linha do TLE bruto (fonte da verdade).</summary>
    public string Line2 { get; private set; } = string.Empty;

    /// <summary>Inclinacao orbital em graus.</summary>
    public double InclinationDegrees { get; private set; }

    /// <summary>Excentricidade da orbita.</summary>
    public double Eccentricity { get; private set; }

    /// <summary>Movimento medio em revolucoes por dia.</summary>
    public double MeanMotionRevsPerDay { get; private set; }

    /// <summary>Altitude media aproximada em km (derivada via leis de Kepler).</summary>
    public double AltitudeKm { get; private set; }

    /// <summary>Faixa orbital em que o detrito se encontra.</summary>
    public DebrisClassification Classification { get; private set; }

    /// <summary>Momento em que o registro foi capturado/ingerido (UTC).</summary>
    public DateTime CapturedAt { get; private set; }

    /// <summary>Chave estrangeira para o aglomerado ao qual o detrito pertence (nulo = ruido/nao agrupado).</summary>
    public Guid? ClusterId { get; private set; }

    /// <summary>Aglomerado ao qual o detrito pertence, se houver.</summary>
    public Cluster? Cluster { get; private set; }

    // Construtor sem parametros exigido pelo EF Core.
    private Debris()
    {
    }

    private Debris(
        int noradId,
        string name,
        string line1,
        string line2,
        OrbitalElements elements,
        DateTime capturedAt)
    {
        Id = Guid.NewGuid();
        NoradId = noradId;
        Name = name;
        Line1 = line1;
        Line2 = line2;
        InclinationDegrees = elements.InclinationDegrees;
        Eccentricity = elements.Eccentricity;
        MeanMotionRevsPerDay = elements.MeanMotionRevsPerDay;
        AltitudeKm = elements.AltitudeKm;
        Classification = elements.Classify();
        CapturedAt = capturedAt;
    }

    /// <summary>
    /// Fabrica um detrito a partir dos dados brutos do TLE e dos elementos orbitais derivados.
    /// </summary>
    public static Debris Create(
        int noradId,
        string name,
        string line1,
        string line2,
        OrbitalElements elements,
        DateTime capturedAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"OBJETO-{noradId}";
        }

        return new Debris(noradId, name, line1, line2, elements, capturedAt);
    }

    /// <summary>
    /// Projeta o detrito no espaco de caracteristicas (altitude x inclinacao) usado pelo DBSCAN.
    /// </summary>
    public OrbitalPoint ToOrbitalPoint() => new(Id, AltitudeKm, InclinationDegrees);

    /// <summary>Vincula o detrito a um aglomerado.</summary>
    public void AssignToCluster(Cluster cluster)
    {
        ArgumentNullException.ThrowIfNull(cluster);
        ClusterId = cluster.Id;
        Cluster = cluster;
    }

    /// <summary>Marca o detrito como ruido (nao pertence a nenhum aglomerado).</summary>
    public void MarkAsNoise()
    {
        ClusterId = null;
        Cluster = null;
    }
}
