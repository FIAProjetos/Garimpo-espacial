using Garimpo.Application.Ports;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.UseCases;

public sealed class AcknowledgeAlertUseCase
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcknowledgeAlertUseCase(IAlertRepository alertRepository, IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var alert = await _alertRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new AlertNotFoundException(id);

        alert.Acknowledge(DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
