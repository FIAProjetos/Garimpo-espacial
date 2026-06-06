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

    /// <summary>Lista todos os detritos ingeridos, ordenados por altitude.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DebrisDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DebrisDto>>> List(CancellationToken cancellationToken)
    {
        var debris = await _getDebris.ListAsync(cancellationToken);
        return Ok(debris);
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
