using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Infrastructure.Tle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Garimpo.Infrastructure.ExternalServices;

/// <summary>
/// Adapter que consome o catalogo TLE publico da Celestrak via HTTP (de forma assincrona)
/// e o converte em entidades de dominio com o <see cref="TleParser"/>.
/// A Celestrak nao exige autenticacao; a URL e o grupo padrao vêm do .env / configuracao.
/// </summary>
public sealed class CelestrakTleProvider : ITleProvider
{
    private readonly HttpClient _httpClient;
    private readonly TleParser _parser;
    private readonly ILogger<CelestrakTleProvider> _logger;
    private readonly string _defaultGroup;

    public CelestrakTleProvider(
        HttpClient httpClient,
        TleParser parser,
        IConfiguration configuration,
        ILogger<CelestrakTleProvider> logger)
    {
        _httpClient = httpClient;
        _parser = parser;
        _logger = logger;
        _defaultGroup = configuration["ExternalServices:Celestrak:DefaultGroup"]
            ?? "cosmos-2251-debris";
    }

    public async Task<IReadOnlyList<Debris>> FetchDebrisAsync(
        string? group = null,
        CancellationToken cancellationToken = default)
    {
        string selectedGroup = string.IsNullOrWhiteSpace(group) ? _defaultGroup : group.Trim();
        string requestUri = $"gp.php?GROUP={Uri.EscapeDataString(selectedGroup)}&FORMAT=tle";

        _logger.LogInformation("Buscando catalogo TLE da Celestrak (grupo: {Group}).", selectedGroup);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        var debris = _parser.Parse(content, DateTime.UtcNow);

        _logger.LogInformation("Ingestao concluida: {Count} detritos interpretados.", debris.Count);
        return debris;
    }
}
