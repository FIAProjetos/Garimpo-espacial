using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Services;
using Garimpo.Domain.ValueObjects;

namespace Garimpo.Application.UseCases;

/// <summary>
/// Caso de uso central: executa o DBSCAN sobre o catalogo de detritos para identificar
/// zonas de alta densidade de lixo espacial, regenerando os aglomerados persistidos.
/// </summary>
public sealed class RunClusteringUseCase
{
    private readonly IDebrisRepository _debrisRepository;
    private readonly IClusterRepository _clusterRepository;
    private readonly IClusteringService _clusteringService;
    private readonly EvaluateAlertsUseCase _evaluateAlerts;
    private readonly IUnitOfWork _unitOfWork;

    public RunClusteringUseCase(
        IDebrisRepository debrisRepository,
        IClusterRepository clusterRepository,
        IClusteringService clusteringService,
        EvaluateAlertsUseCase evaluateAlerts,
        IUnitOfWork unitOfWork)
    {
        _debrisRepository = debrisRepository;
        _clusterRepository = clusterRepository;
        _clusteringService = clusteringService;
        _evaluateAlerts = evaluateAlerts;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClusteringResultDto> ExecuteAsync(
        ClusteringRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // Entidades rastreadas para que a reatribuicao de aglomerado seja persistida.
        IReadOnlyList<Debris> allDebris = await _debrisRepository.GetAllTrackedAsync(cancellationToken);

        // Remove os aglomerados da execucao anterior (FK dos detritos volta a nulo).
        await _clusterRepository.ClearAsync(cancellationToken);

        if (allDebris.Count == 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new ClusteringResultDto(0, 0, 0, request.Epsilon, request.MinPoints, DateTime.UtcNow);
        }

        var debrisById = allDebris.ToDictionary(d => d.Id);
        IReadOnlyList<OrbitalPoint> points = allDebris.Select(d => d.ToOrbitalPoint()).ToList();

        IReadOnlyDictionary<Guid, int> assignments =
            _clusteringService.Cluster(points, request.Epsilon, request.MinPoints);

        var membersByLabel = new Dictionary<int, List<Debris>>();
        int noiseCount = 0;

        foreach (var (debrisId, label) in assignments)
        {
            Debris debris = debrisById[debrisId];

            if (label == DbscanClusteringService.NoiseLabel)
            {
                debris.MarkAsNoise();
                noiseCount++;
                continue;
            }

            if (!membersByLabel.TryGetValue(label, out var members))
            {
                members = new List<Debris>();
                membersByLabel[label] = members;
            }

            members.Add(debris);
        }

        DateTime now = DateTime.UtcNow;
        var clusters = membersByLabel
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => Cluster.Create(kvp.Key, kvp.Value, now))
            .ToList();

        if (clusters.Count > 0)
        {
            await _clusterRepository.AddRangeAsync(clusters, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        int alertsGenerated = await _evaluateAlerts.ExecuteAsync(cancellationToken: cancellationToken);

        return new ClusteringResultDto(
            ProcessedDebris: allDebris.Count,
            ClustersFound: clusters.Count,
            NoiseCount: noiseCount,
            Epsilon: request.Epsilon,
            MinPoints: request.MinPoints,
            CompletedAt: now,
            AlertsGenerated: alertsGenerated);
    }
}
