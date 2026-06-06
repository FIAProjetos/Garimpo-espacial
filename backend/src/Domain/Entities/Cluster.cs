namespace Garimpo.Domain.Entities;

/// <summary>
/// Entidade processada e enriquecida: agrupa multiplos detritos em uma zona classificada
/// de alta densidade de lixo espacial, conforme resultado do algoritmo DBSCAN.
/// </summary>
public class Cluster
{
    private readonly List<Debris> _members = new();

    /// <summary>Identidade do aglomerado.</summary>
    public Guid Id { get; private set; }

    /// <summary>Rotulo sequencial atribuido pelo DBSCAN (0, 1, 2, ...).</summary>
    public int Label { get; private set; }

    /// <summary>Altitude media (km) do centro do aglomerado.</summary>
    public double CentroidAltitudeKm { get; private set; }

    /// <summary>Inclinacao media (graus) do centro do aglomerado.</summary>
    public double CentroidInclinationDegrees { get; private set; }

    /// <summary>Quantidade de detritos no aglomerado.</summary>
    public int MemberCount { get; private set; }

    /// <summary>
    /// Densidade relativa do aglomerado: membros por unidade de "area" do espaco de
    /// caracteristicas (proxy de risco de colisao na zona).
    /// </summary>
    public double Density { get; private set; }

    /// <summary>Momento de geracao do aglomerado (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Detritos que compoem o aglomerado.</summary>
    public IReadOnlyCollection<Debris> Members => _members.AsReadOnly();

    // Construtor sem parametros exigido pelo EF Core.
    private Cluster()
    {
    }

    private Cluster(int label, IReadOnlyCollection<Debris> members, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        Label = label;
        CreatedAt = createdAt;
        AddMembers(members);
    }

    /// <summary>
    /// Cria um aglomerado a partir dos detritos membros, calculando centroide e densidade.
    /// </summary>
    public static Cluster Create(int label, IReadOnlyCollection<Debris> members, DateTime createdAt)
    {
        ArgumentNullException.ThrowIfNull(members);

        if (members.Count == 0)
        {
            throw new ArgumentException("Um aglomerado precisa de ao menos um membro.", nameof(members));
        }

        return new Cluster(label, members, createdAt);
    }

    private void AddMembers(IReadOnlyCollection<Debris> members)
    {
        foreach (var member in members)
        {
            member.AssignToCluster(this);
            _members.Add(member);
        }

        MemberCount = _members.Count;
        RecalculateCentroidAndDensity();
    }

    private void RecalculateCentroidAndDensity()
    {
        CentroidAltitudeKm = _members.Average(m => m.AltitudeKm);
        CentroidInclinationDegrees = _members.Average(m => m.InclinationDegrees);

        double altitudeSpread = _members.Max(m => m.AltitudeKm) - _members.Min(m => m.AltitudeKm);
        double inclinationSpread = _members.Max(m => m.InclinationDegrees) - _members.Min(m => m.InclinationDegrees);

        // Area minima de 1.0 evita divisao por zero quando todos os pontos coincidem.
        double area = Math.Max(1.0, altitudeSpread * inclinationSpread);
        Density = _members.Count / area;
    }
}
