namespace Garimpo.Application.Dtos;

public sealed record AuthResponseDto(string Token, DateTime ExpiresAt, UserDto User);
