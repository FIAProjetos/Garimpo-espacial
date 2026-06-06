using Garimpo.Domain.Entities;

namespace Garimpo.Application.Ports;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
