using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Application.Features.WikiPages.Commands;
using WikiArea.Application.Features.WikiPages.Queries;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WikiPagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WikiPagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a wiki page by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<WikiPageDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetWikiPageByIdQuery(id));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get a wiki page by slug
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<WikiPageDto>> GetBySlug(string slug)
    {
        var result = await _mediator.Send(new GetWikiPageBySlugQuery(slug));
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Search wiki pages
    /// </summary>
    [HttpGet("search")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<WikiPageSummaryDto>>> Search([FromQuery] string term)
    {
        var result = await _mediator.Send(new SearchWikiPagesQuery(term));
        return Ok(result);
    }

    /// <summary>
    /// Get pages by folder
    /// </summary>
    [HttpGet("by-folder/{folderId?}")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<WikiPageSummaryDto>>> GetByFolder(string? folderId = null)
    {
        var result = await _mediator.Send(new GetWikiPagesByFolderQuery(folderId));
        return Ok(result);
    }

    /// <summary>
    /// Get recently updated pages
    /// </summary>
    [HttpGet("recent")]
    [Authorize(Policy = "RequireReaderRole")]
    public async Task<ActionResult<IEnumerable<WikiPageSummaryDto>>> GetRecent([FromQuery] int count = 10)
    {
        var result = await _mediator.Send(new GetRecentWikiPagesQuery(count));
        return Ok(result);
    }

    /// <summary>
    /// Create a new wiki page
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiPageDto>> Create(CreateWikiPageCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing wiki page
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiPageDto>> Update(string id, UpdateWikiPageCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a wiki page
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteWikiPageCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Publish a wiki page
    /// </summary>
    [HttpPost("{id}/publish")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiPageDto>> Publish(string id)
    {
        var result = await _mediator.Send(new PublishWikiPageCommand(id));
        return Ok(result);
    }

    /// <summary>
    /// Submit a wiki page for review
    /// </summary>
    [HttpPost("{id}/submit-for-review")]
    [Authorize(Policy = "RequireWriterRole")]
    public async Task<ActionResult<WikiPageDto>> SubmitForReview(string id)
    {
        var result = await _mediator.Send(new SubmitWikiPageForReviewCommand(id));
        return Ok(result);
    }

    /// <summary>
    /// Approve a wiki page review
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Policy = "RequireReviewerRole")]
    public async Task<ActionResult<WikiPageDto>> ApproveReview(string id, [FromBody] string notes = "")
    {
        var result = await _mediator.Send(new ApproveWikiPageReviewCommand(id, notes));
        return Ok(result);
    }

    /// <summary>
    /// Reject a wiki page review
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Policy = "RequireReviewerRole")]
    public async Task<ActionResult<WikiPageDto>> RejectReview(string id, [FromBody] string notes)
    {
        var result = await _mediator.Send(new RejectWikiPageReviewCommand(id, notes));
        return Ok(result);
    }
} 