using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.UseCases;

public sealed class LoginUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = User.NormalizeEmail(request.Email);
        var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        (string token, DateTime expiresAt) = _tokenService.GenerateToken(user);

        return new AuthResponseDto(token, expiresAt, UserDto.FromEntity(user));
    }
}
