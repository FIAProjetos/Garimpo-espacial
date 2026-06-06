using System.Reflection;
using Garimpo.Api.Middleware;
using Garimpo.Api.Security;
using Garimpo.Application;
using Garimpo.Infrastructure;
using Garimpo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// Carrega .env do mono-repo (sobrescreve appsettings.json — secrets fora do Git)
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

ValidateRequiredConfiguration(builder.Configuration, builder.Environment);

const string CorsPolicy = "GarimpoCors";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, null);

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = 60;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("write", limiter =>
    {
        limiter.PermitLimit = 10;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Garimpo Espacial API",
        Version = "v1",
        Description = "Plataforma de inteligencia de detritos espaciais com controles de seguranca "
                    + "(API Key, rate limiting, auditoria) para operacoes criticas de missao orbital."
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key de operador. Header: X-Api-Key",
        Name = ApiKeyAuthenticationHandler.HeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        string[] allowedOrigins = builder.Configuration
            .GetSection("Security:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:8081", "http://localhost:19000"];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await ApplyMigrationsAsync(app);

app.UseSecurityHeaders();
app.UseExceptionHandlingMiddleware();
app.UseAuditLogging();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Garimpo Espacial API v1");
    options.RoutePrefix = "swagger";
});

app.UseRateLimiter();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("api");

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GarimpoDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (!context.Database.IsRelational())
    {
        return;
    }

    const int maxAttempts = 10;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(
                "Banco de dados indisponivel (tentativa {Attempt}/{Max}): {Message}. Nova tentativa em 3s.",
                attempt, maxAttempts, ex.Message);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

static void ValidateRequiredConfiguration(IConfiguration configuration, IHostEnvironment environment)
{
    var missing = new List<string>();

    if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
    {
        missing.Add("ConnectionStrings:DefaultConnection");
    }

    if (string.IsNullOrWhiteSpace(configuration["Security:ApiKey"]))
    {
        missing.Add("Security:ApiKey");
    }

    if (missing.Count == 0)
    {
        return;
    }

    string message =
        "Configuracao obrigatoria ausente: " + string.Join(", ", missing)
        + ". Copie .env.example para .env na raiz do mono-repo e preencha os valores.";

    if (environment.IsProduction())
    {
        throw new InvalidOperationException(message);
    }

    Console.WriteLine($"AVISO: {message}");
}

public partial class Program;
