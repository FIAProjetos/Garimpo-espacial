using Garimpo.Domain.Entities.Alerts;

namespace Garimpo.Application.Dtos;

public sealed record AlertDto(
    Guid Id,
    string AlertType,
    string Severity,
    string Message,
    bool RequiresImmediateAction,
    bool IsAcknowledged,
    DateTime TriggeredAt,
    DateTime? AcknowledgedAt)
{
    public static AlertDto FromEntity(Alert alert) => new(
        alert.Id,
        alert.GetAlertType(),
        alert.Severity.ToString(),
        alert.BuildMessage(),
        alert.RequiresImmediateAction(),
        alert.IsAcknowledged,
        alert.TriggeredAt,
        alert.AcknowledgedAt);
}
