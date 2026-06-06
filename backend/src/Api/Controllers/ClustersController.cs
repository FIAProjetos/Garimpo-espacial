using Garimpo.Application.Dtos;
using Garimpo.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Garimpo.Api.Controllers;

/// <summary>Geracao e consulta de aglomerados (zonas de alta densidade de lixo espacial).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ClustersController : ControllerBase
{
    private readonly GetClustersUseCase _getClusters;
    private readonly RunClusteringUseCase _runClustering;

    public ClustersController(GetClustersUseCase getClusters, RunClusteringUseCase runClustering)
    {
        _getClusters = getClusters;
        _runClustering = runClustering;
    }

    /// <summary>Lista aglomerados com paginacao, ordenados por densidade (maior risco primeiro).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ClusterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ClusterDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _getClusters.ListPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Executa o algoritmo DBSCAN sobre o catalogo atual, regenerando os aglomerados.
    /// </summary>
    [HttpPost("run")]
    [Authorize]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(ClusteringResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClusteringResultDto>> Run(
        [FromBody] ClusteringRequestDto? request,
        CancellationToken cancellationToken)
    {
        var result = await _runClustering.ExecuteAsync(request ?? new ClusteringRequestDto(), cancellationToken);
        return Ok(result);
    }
}
