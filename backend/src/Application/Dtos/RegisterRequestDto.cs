namespace Garimpo.Application.Dtos;

public sealed record RegisterRequestDto(string Email, string Password, string FullName);
