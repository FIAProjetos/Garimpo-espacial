namespace Garimpo.Domain.Exceptions;

public sealed class EmailAlreadyRegisteredException : DomainException
{
    public EmailAlreadyRegisteredException(string email)
        : base($"O email '{email}' ja esta cadastrado.")
    {
    }
}
