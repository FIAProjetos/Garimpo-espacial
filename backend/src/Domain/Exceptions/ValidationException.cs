namespace Garimpo.Domain.Exceptions;

/// <summary>
/// Entrada HTTP invalida (formato, tamanho ou conteudo perigoso). Mitiga injecao e XSS em camadas de aplicacao.
/// </summary>
public sealed class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}
