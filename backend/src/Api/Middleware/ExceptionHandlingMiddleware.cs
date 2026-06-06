using System.Text.Json;
using Garimpo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Garimpo.Api.Middleware;

/// <summary>
/// Middleware que traduz excecoes em respostas ProblemDetails padronizadas,
/// garantindo que o sistema critico nao quebre abruptamente e devolva erros claros.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            DebrisNotFoundException => (StatusCodes.Status404NotFound, "Recurso nao encontrado"),
            AlertNotFoundException => (StatusCodes.Status404NotFound, "Alerta nao encontrado"),
            TleParsingException => (StatusCodes.Status422UnprocessableEntity, "Dados TLE invalidos"),
            DomainException => (StatusCodes.Status400BadRequest, "Violacao de regra de negocio"),
            HttpRequestException => (StatusCodes.Status502BadGateway, "Falha ao consultar a fonte externa de TLE"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
            OperationCanceledException => (499, "Requisicao cancelada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno inesperado")
        };

        if (status >= 500)
        {
            _logger.LogError(exception, "Erro nao tratado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Requisicao rejeitada ({Status}): {Message}", status, exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}

/// <summary>Extensoes para registrar o middleware no pipeline.</summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
