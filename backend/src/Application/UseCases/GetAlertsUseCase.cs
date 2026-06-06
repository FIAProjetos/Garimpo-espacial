using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.UseCases;

public sealed class GetAlertsUseCase
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertsUseCase(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<IReadOnlyList<AlertDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var alerts = await _alertRepository.GetAllAsync(cancellationToken);

        return alerts
            .OrderByDescending(a => a.Severity)
            .ThenByDescending(a => a.TriggeredAt)
            .Select(AlertDto.FromEntity)
            .ToList();
    }

}
