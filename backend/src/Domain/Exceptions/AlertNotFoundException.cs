namespace Garimpo.Domain.Exceptions;

public sealed class AlertNotFoundException : DomainException
{
    public AlertNotFoundException(Guid id)
        : base($"Alerta com identificador '{id}' nao foi encontrado.")
    {
    }
}
