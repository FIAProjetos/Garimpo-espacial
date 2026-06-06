namespace Garimpo.Domain.Exceptions;

/// <summary>
/// Lancada quando um detrito solicitado nao existe no catalogo.
/// </summary>
public sealed class DebrisNotFoundException : DomainException
{
    public DebrisNotFoundException(Guid id)
        : base($"Detrito com identificador '{id}' nao foi encontrado no catalogo.")
    {
    }

    public DebrisNotFoundException(int noradId)
        : base($"Detrito com NORAD ID '{noradId}' nao foi encontrado no catalogo.")
    {
    }
}
