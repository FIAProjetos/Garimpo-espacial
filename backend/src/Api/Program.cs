using System.Reflection;
using System.Text;
using Garimpo.Api.Middleware;
using Garimpo.Application;
using Garimpo.Infrastructure;
using Garimpo.Infrastructure.Persistence;
using Garimpo.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

ValidateRequiredConfiguration(builder.Configuration, builder.Environment);

const string CorsPolicy = "GarimpoCors";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

string jwtSecret = builder.Configuration["Security:Jwt:Secret"]!;
string jwtIssuer = builder.Configuration["Security:Jwt:Issuer"] ?? "GarimpoEspacial";
string jwtAudience = builder.Configuration["Security:Jwt:Audience"] ?? "GarimpoEspacialApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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
        Description = "Plataforma de inteligencia de detritos espaciais. "
                    + "Autentique via POST /api/auth/login e use o JWT como Bearer token."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer. Obtenha em POST /api/auth/login",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
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
await SeedDatabaseAsync(app);

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

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

static void ValidateRequiredConfiguration(IConfiguration configuration, IHostEnvironment environment)
{
    var missing = new List<string>();

    if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
    {
        missing.Add("ConnectionStrings:DefaultConnection");
    }

    string? jwtSecret = configuration["Security:Jwt:Secret"];
    if (string.IsNullOrWhiteSpace(jwtSecret))
    {
        missing.Add("Security:Jwt:Secret");
    }
    else if (jwtSecret.Length < 32)
    {
        missing.Add("Security:Jwt:Secret (minimo 32 caracteres)");
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
