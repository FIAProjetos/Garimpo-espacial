using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;

namespace Garimpo.Application.UseCases;

/// <summary>
/// Caso de uso: ingerir o catalogo TLE de detritos a partir de uma fonte externa,
/// deduplicando contra o que ja existe no banco.
/// </summary>
public sealed class IngestTleUseCase
{
    private readonly ITleProvider _tleProvider;
    private readonly IDebrisRepository _debrisRepository;
    private readonly EvaluateAlertsUseCase _evaluateAlerts;
    private readonly IUnitOfWork _unitOfWork;

    public IngestTleUseCase(
        ITleProvider tleProvider,
        IDebrisRepository debrisRepository,
        EvaluateAlertsUseCase evaluateAlerts,
        IUnitOfWork unitOfWork)
    {
        _tleProvider = tleProvider;
        _debrisRepository = debrisRepository;
        _evaluateAlerts = evaluateAlerts;
        _unitOfWork = unitOfWork;
    }

    public async Task<IngestionResultDto> ExecuteAsync(
        string? group = null,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Debris> fetched = await _tleProvider.FetchDebrisAsync(group, cancellationToken);

        IReadOnlySet<int> existingNoradIds = await _debrisRepository.GetExistingNoradIdsAsync(cancellationToken);

        var toImport = new List<Debris>();
        var seenInBatch = new HashSet<int>();

        foreach (var debris in fetched)
        {
            if (existingNoradIds.Contains(debris.NoradId) || !seenInBatch.Add(debris.NoradId))
            {
                continue;
            }

            toImport.Add(debris);
        }

        if (toImport.Count > 0)
        {
            await _debrisRepository.AddRangeAsync(toImport, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        int skipped = fetched.Count - toImport.Count;
        if (skipped > 0)
        {
            await _evaluateAlerts.RegisterTelemetryAlertAsync(
                "celestrak-tle", fetched.Count, skipped, cancellationToken);
        }

        return new IngestionResultDto(
            Fetched: fetched.Count,
            Imported: toImport.Count,
            Skipped: fetched.Count - toImport.Count,
            CompletedAt: DateTime.UtcNow);
    }
}
