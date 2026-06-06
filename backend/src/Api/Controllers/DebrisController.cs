using Garimpo.Application.Dtos;
using Garimpo.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Garimpo.Api.Controllers;

/// <summary>Consulta do catalogo de detritos orbitais.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class DebrisController : ControllerBase
{
    private readonly GetDebrisUseCase _getDebris;

    public DebrisController(GetDebrisUseCase getDebris)
    {
        _getDebris = getDebris;
    }

    /// <summary>Lista detritos ingeridos com paginacao, ordenados por altitude.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<DebrisDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<DebrisDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _getDebris.ListPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtem um detrito especifico pelo identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DebrisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DebrisDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var debris = await _getDebris.GetByIdAsync(id, cancellationToken);
        return Ok(debris);
    }
}
