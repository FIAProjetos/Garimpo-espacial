using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.UseCases;

/// <summary>
/// Caso de uso de leitura: lista os detritos ou obtem um detrito especifico.
/// </summary>
public sealed class GetDebrisUseCase
{
    private readonly IDebrisRepository _debrisRepository;

    public GetDebrisUseCase(IDebrisRepository debrisRepository)
    {
        _debrisRepository = debrisRepository;
    }

    public async Task<IReadOnlyList<DebrisDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var debris = await _debrisRepository.GetAllAsync(cancellationToken);

        return debris
            .OrderBy(d => d.AltitudeKm)
            .Select(DebrisDto.FromEntity)
            .ToList();
    }

    /// <exception cref="DebrisNotFoundException">Quando o detrito nao existe.</exception>
    public async Task<DebrisDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var debris = await _debrisRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new DebrisNotFoundException(id);

        return DebrisDto.FromEntity(debris);
    }
}
