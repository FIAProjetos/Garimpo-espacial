using System.Diagnostics;
using System.Security.Claims;

namespace Garimpo.Api.Middleware;

/// <summary>
/// Registra eventos de auditoria para operacoes sensiveis (escrita, autenticacao),
/// suportando monitoramento de logs exigido pela disciplina de Cybersecurity.
/// </summary>
public sealed class AuditLoggingMiddleware
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST", "PUT", "PATCH", "DELETE"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!AuditedMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        string actor = context.User.FindFirstValue(ClaimTypes.Name) ?? "anonymous";
        string path = context.Request.Path;
        string method = context.Request.Method;
        string clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "AUDIT actor={Actor} method={Method} path={Path} status={Status} ip={Ip} durationMs={Duration}",
            actor, method, path, context.Response.StatusCode, clientIp, stopwatch.ElapsedMilliseconds);
    }
}

public static class AuditLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder app)
        => app.UseMiddleware<AuditLoggingMiddleware>();
}
