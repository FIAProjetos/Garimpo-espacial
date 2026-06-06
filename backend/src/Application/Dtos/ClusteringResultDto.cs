namespace Garimpo.Application.Dtos;

/// <summary>
/// Resumo do resultado de uma execucao de clusterizacao DBSCAN.
/// </summary>
public sealed record ClusteringResultDto(
    int ProcessedDebris,
    int ClustersFound,
    int NoiseCount,
    double Epsilon,
    int MinPoints,
    DateTime CompletedAt,
    int AlertsGenerated = 0);
