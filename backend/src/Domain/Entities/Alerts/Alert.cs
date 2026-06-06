using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.Alerts;

/// <summary>
/// Classe abstrata para alertas de missao orbital. Sistemas criticos espaciais precisam
/// de notificacoes polimorficas que variam conforme o tipo de ameaca detectada.
/// </summary>
public abstract class Alert
{
    public Guid Id { get; protected set; }
    public AlertSeverity Severity { get; protected set; }
    public DateTime TriggeredAt { get; protected set; }
    public bool IsAcknowledged { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }

    /// <summary>Tipo discriminador para persistencia e serializacao.</summary>
    public abstract string GetAlertType();

    /// <summary>Mensagem polimorfica descritiva do alerta.</summary>
    public abstract string BuildMessage();

    /// <summary>Indica se o alerta exige acao imediata do analista de missao.</summary>
    public abstract bool RequiresImmediateAction();

    public void Acknowledge(DateTime acknowledgedAt)
    {
        IsAcknowledged = true;
        AcknowledgedAt = acknowledgedAt;
    }
}
