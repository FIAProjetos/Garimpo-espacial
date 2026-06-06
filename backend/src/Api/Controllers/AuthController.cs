using Garimpo.Application.Dtos;
using Garimpo.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garimpo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUser;
    private readonly LoginUserUseCase _loginUser;

    public AuthController(RegisterUserUseCase registerUser, LoginUserUseCase loginUser)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
    }

    /// <summary>Cadastra um novo usuario (analista de missao orbital).</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await _registerUser.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Register), user);
    }

    /// <summary>Autentica usuario e retorna JWT Bearer.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _loginUser.ExecuteAsync(request, cancellationToken);
        return Ok(response);
    }
}
