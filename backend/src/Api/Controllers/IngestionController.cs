using Garimpo.Application.Dtos;
using Garimpo.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Garimpo.Api.Controllers;

/// <summary>Disparo da ingestao do catalogo TLE a partir da fonte externa (Celestrak).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class IngestionController : ControllerBase
{
    private readonly IngestTleUseCase _ingestTle;

    public IngestionController(IngestTleUseCase ingestTle)
    {
        _ingestTle = ingestTle;
    }

    /// <summary>
    /// Busca o catalogo TLE da Celestrak e importa os detritos novos.
    /// </summary>
    /// <param name="group">
    /// Grupo Celestrak (ex.: "cosmos-2251-debris", "iridium-33-debris", "active").
    /// Se omitido, usa o grupo padrao de detritos em LEO.
    /// </param>
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(IngestionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<IngestionResultDto>> Ingest(
        [FromQuery] string? group,
        CancellationToken cancellationToken)
    {
        var result = await _ingestTle.ExecuteAsync(group, cancellationToken);
        return Ok(result);
    }
}
