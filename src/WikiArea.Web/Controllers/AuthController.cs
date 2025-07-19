using MediatR;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Application.Features.Auth.Commands;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
    {
        var command = new SignupCommand
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            DisplayName = request.DisplayName,
            Department = request.Department
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("signin")]
    public async Task<ActionResult<AuthResponse>> Signin([FromBody] SigninRequest request)
    {
        var command = new SigninCommand
        {
            UsernameOrEmail = request.UsernameOrEmail,
            Password = request.Password
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
} 