using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Application.Features.Users.Commands;
using WikiArea.Application.Features.Users.Queries;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Search users
    /// </summary>
    [HttpGet("search")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> Search([FromQuery] string term)
    {
        var result = await _mediator.Send(new SearchUsersQuery(term));
        return Ok(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdministratorRole")]
    public async Task<ActionResult<UserDto>> Create(CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
} 