using Garimpo.Domain.Entities;

namespace Garimpo.Application.Dtos;

public sealed record UserDto(Guid Id, string Email, string FullName, string Role, DateTime CreatedAt)
{
    public static UserDto FromEntity(User user) => new(
        user.Id,
        user.Email,
        user.FullName,
        user.Role.ToString(),
        user.CreatedAt);
}
