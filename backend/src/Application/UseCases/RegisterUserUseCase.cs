using Garimpo.Application.Dtos;
using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.UseCases;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> ExecuteAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        string normalizedEmail = User.NormalizeEmail(request.Email);

        if (await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            throw new EmailAlreadyRegisteredException(normalizedEmail);
        }

        string passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(normalizedEmail, passwordHash, request.FullName);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UserDto.FromEntity(user);
    }
}
