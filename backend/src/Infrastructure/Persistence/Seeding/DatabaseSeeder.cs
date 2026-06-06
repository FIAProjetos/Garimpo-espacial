using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Garimpo.Infrastructure.Persistence.Seeding;

/// <summary>
/// Popula dados iniciais obrigatorios para avaliacao (usuario de teste FIAP).
/// Idempotente: nao duplica registros se ja existirem.
/// </summary>
public sealed class DatabaseSeeder
{
    public const string TestUserEmail = "fiap@teste.com";
    public const string TestUserPassword = "123456";
    public const string TestUserFullName = "Usuario Teste FIAP";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<DatabaseSeeder> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedUserIfNotExistsAsync(
            TestUserEmail,
            TestUserPassword,
            TestUserFullName,
            UserRole.Analyst,
            cancellationToken);
    }

    private async Task SeedUserIfNotExistsAsync(
        string email,
        string password,
        string fullName,
        UserRole role,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            _logger.LogInformation("Seed: usuario {Email} ja existe, pulando.", email);
            return;
        }

        string passwordHash = _passwordHasher.Hash(password);
        var user = User.Create(email, passwordHash, fullName, role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seed: usuario {Email} criado.", email);
    }
}
