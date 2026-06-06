using Garimpo.Domain.Entities.Alerts;

namespace Garimpo.Application.Ports;

public interface IAlertRepository
{
    Task AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Alert>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Alert?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken = default);
}
