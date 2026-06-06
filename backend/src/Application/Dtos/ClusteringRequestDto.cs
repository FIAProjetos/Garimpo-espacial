namespace Garimpo.Application.Dtos;

/// <summary>
/// Parametros de entrada para uma execucao do DBSCAN.
/// </summary>
public sealed record ClusteringRequestDto
{
    /// <summary>Raio de vizinhanca em desvios-padrao. Padrao: 0.3.</summary>
    public double Epsilon { get; init; } = 0.3;

    /// <summary>Minimo de vizinhos para formar um nucleo de cluster. Padrao: 5.</summary>
    public int MinPoints { get; init; } = 5;
}
