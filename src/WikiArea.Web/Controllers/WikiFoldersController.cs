using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Application.Features.WikiFolders.Commands;
using WikiArea.Application.Features.WikiFolders.Queries;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikiFoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WikiFoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a wiki folder by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<WikiFolderDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetWikiFolderByIdQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get root folders
    /// </summary>
    [HttpGet("root")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<WikiFolderDto>>> GetRootFolders()
    {
        var result = await _mediator.Send(new GetRootFoldersQuery());
        return Ok(result);
    }

    /// <summary>
    /// Create a new wiki folder
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiFolderDto>> Create(CreateWikiFolderCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing wiki folder
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiFolderDto>> Update(string id, UpdateWikiFolderCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }
} 