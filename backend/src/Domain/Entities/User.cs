using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public static User Create(string email, string passwordHash, string fullName, UserRole role = UserRole.Analyst)
    {
        string normalizedEmail = NormalizeEmail(email);

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new ArgumentException("Email e obrigatorio.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Hash de senha e obrigatorio.", nameof(passwordHash));
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            FullName = string.IsNullOrWhiteSpace(fullName) ? normalizedEmail : fullName.Trim(),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static string NormalizeEmail(string email)
        => email.Trim().ToLowerInvariant();
}
