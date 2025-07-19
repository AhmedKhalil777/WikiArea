using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Application.Features.Comments.Commands;
using WikiArea.Application.Features.Comments.Queries;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a comment by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<CommentDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetCommentByIdQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get comments for a specific wiki page
    /// </summary>
    [HttpGet("page/{pageId}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByPage(string pageId)
    {
        var result = await _mediator.Send(new GetCommentsByPageQuery(pageId));
        return Ok(result);
    }

    /// <summary>
    /// Create a new comment
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<CommentDto>> Create(CreateCommentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing comment
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<CommentDto>> Update(string id, UpdateCommentCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Resolve a comment
    /// </summary>
    [HttpPost("{id}/resolve")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<CommentDto>> Resolve(string id)
    {
        var result = await _mediator.Send(new ResolveCommentCommand(id));
        return Ok(result);
    }
} 