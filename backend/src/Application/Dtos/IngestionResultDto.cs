namespace Garimpo.Application.Dtos;

/// <summary>
/// Resumo do resultado de uma ingestao de TLE.
/// </summary>
public sealed record IngestionResultDto(
    int Fetched,
    int Imported,
    int Skipped,
    DateTime CompletedAt);
