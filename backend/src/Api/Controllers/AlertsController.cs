using Garimpo.Application.Dtos;
using Garimpo.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Garimpo.Api.Controllers;

/// <summary>Consulta e reconhecimento de alertas de risco orbital.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AlertsController : ControllerBase
{
    private readonly GetAlertsUseCase _getAlerts;
    private readonly EvaluateAlertsUseCase _evaluateAlerts;
    private readonly AcknowledgeAlertUseCase _acknowledgeAlert;

    public AlertsController(
        GetAlertsUseCase getAlerts,
        EvaluateAlertsUseCase evaluateAlerts,
        AcknowledgeAlertUseCase acknowledgeAlert)
    {
        _getAlerts = getAlerts;
        _evaluateAlerts = evaluateAlerts;
        _acknowledgeAlert = acknowledgeAlert;
    }

    /// <summary>Lista alertas ativos, ordenados por severidade.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlertDto>>> List(CancellationToken cancellationToken)
    {
        return Ok(await _getAlerts.ListAsync(cancellationToken));
    }

    /// <summary>Reavalia aglomerados e regenera alertas de densidade.</summary>
    [HttpPost("evaluate")]
    [Authorize]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Evaluate(CancellationToken cancellationToken)
    {
        int count = await _evaluateAlerts.ExecuteAsync(cancellationToken: cancellationToken);
        return Ok(new { alertsGenerated = count });
    }

    /// <summary>Reconhece um alerta (acao do analista de missao).</summary>
    [HttpPost("{id:guid}/acknowledge")]
    [Authorize]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Acknowledge(Guid id, CancellationToken cancellationToken)
    {
        await _acknowledgeAlert.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
