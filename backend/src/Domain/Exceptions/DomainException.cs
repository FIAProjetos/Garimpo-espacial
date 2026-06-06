namespace Garimpo.Domain.Exceptions;

/// <summary>
/// Excecao base para erros de regra de negocio do dominio. Sistemas criticos espaciais
/// nao podem quebrar abruptamente: usamos excecoes especificas para tratamento controlado.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
