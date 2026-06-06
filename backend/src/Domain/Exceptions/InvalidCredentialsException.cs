namespace Garimpo.Domain.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Email ou senha invalidos.")
    {
    }
}
