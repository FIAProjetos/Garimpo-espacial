using System.Text.RegularExpressions;
using Garimpo.Domain.Exceptions;

namespace Garimpo.Application.Validation;

/// <summary>
/// Validacao centralizada de entradas da API (defesa em profundidade contra SQLi, XSS e abuso).
/// </summary>
public static class InputValidators
{
    private const int MaxEmailLength = 256;
    private const int MaxPasswordLength = 128;
    private const int MaxFullNameLength = 128;
    private const int MinPasswordLength = 6;
    private const int MaxPageSize = 200;
    private const int MaxPage = 10_000;

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex DangerousMarkupRegex = new(
        @"<|>|javascript\s*:|data\s*:|vbscript\s*:|on\w+\s*=",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> AllowedCelestrakGroups = new(StringComparer.OrdinalIgnoreCase)
    {
        "cosmos-2251-debris",
        "iridium-33-debris",
        "active",
    };

    public static string ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ValidationException("Email e obrigatorio.");
        }

        string trimmed = email.Trim();

        if (trimmed.Length > MaxEmailLength)
        {
            throw new ValidationException($"Email excede {MaxEmailLength} caracteres.");
        }

        RejectDangerousContent(trimmed, "Email");

        if (!EmailRegex.IsMatch(trimmed))
        {
            throw new ValidationException("Formato de email invalido.");
        }

        return trimmed.ToLowerInvariant();
    }

    public static string ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ValidationException("Senha e obrigatoria.");
        }

        if (password.Length < MinPasswordLength)
        {
            throw new ValidationException($"Senha deve ter no minimo {MinPasswordLength} caracteres.");
        }

        if (password.Length > MaxPasswordLength)
        {
            throw new ValidationException($"Senha excede {MaxPasswordLength} caracteres.");
        }

        if (password.Any(c => char.IsControl(c)))
        {
            throw new ValidationException("Senha contem caracteres de controle invalidos.");
        }

        return password;
    }

    public static string ValidateFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ValidationException("Nome completo e obrigatorio.");
        }

        string trimmed = fullName.Trim();

        if (trimmed.Length > MaxFullNameLength)
        {
            throw new ValidationException($"Nome completo excede {MaxFullNameLength} caracteres.");
        }

        RejectDangerousContent(trimmed, "Nome completo");

        if (trimmed.Any(c => char.IsControl(c)))
        {
            throw new ValidationException("Nome completo contem caracteres de controle invalidos.");
        }

        return trimmed;
    }

    public static string? ValidateCelestrakGroup(string? group)
    {
        if (string.IsNullOrWhiteSpace(group))
        {
            return null;
        }

        string trimmed = group.Trim();

        if (trimmed.Length > 64)
        {
            throw new ValidationException("Grupo Celestrak excede 64 caracteres.");
        }

        RejectDangerousContent(trimmed, "Grupo Celestrak");

        if (!AllowedCelestrakGroups.Contains(trimmed))
        {
            throw new ValidationException(
                "Grupo Celestrak nao permitido. Valores aceitos: cosmos-2251-debris, iridium-33-debris, active.");
        }

        return trimmed;
    }

    public static void ValidateClusteringParameters(double epsilon, int minPoints)
    {
        if (double.IsNaN(epsilon) || double.IsInfinity(epsilon))
        {
            throw new ValidationException("Epsilon deve ser um numero finito.");
        }

        if (epsilon is < 0.01 or > 5.0)
        {
            throw new ValidationException("Epsilon deve estar entre 0.01 e 5.0.");
        }

        if (minPoints is < 2 or > 100)
        {
            throw new ValidationException("MinPoints deve estar entre 2 e 100.");
        }
    }

    public static (int Page, int PageSize) ValidatePagination(int page, int pageSize)
    {
        if (page < 1 || page > MaxPage)
        {
            throw new ValidationException($"Pagina deve estar entre 1 e {MaxPage}.");
        }

        if (pageSize < 1 || pageSize > MaxPageSize)
        {
            throw new ValidationException($"PageSize deve estar entre 1 e {MaxPageSize}.");
        }

        return (page, pageSize);
    }

    private static void RejectDangerousContent(string value, string fieldName)
    {
        if (DangerousMarkupRegex.IsMatch(value))
        {
            throw new ValidationException($"{fieldName} contem padroes nao permitidos (HTML/script).");
        }
    }
}
