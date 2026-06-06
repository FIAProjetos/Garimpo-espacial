using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Application.Validation;
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
        string normalizedEmail = InputValidators.ValidateEmail(request.Email);
        string password = InputValidators.ValidatePassword(request.Password);
        var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        (string token, DateTime expiresAt) = _tokenService.GenerateToken(user);

        return new AuthResponseDto(token, expiresAt, UserDto.FromEntity(user));
    }
}
