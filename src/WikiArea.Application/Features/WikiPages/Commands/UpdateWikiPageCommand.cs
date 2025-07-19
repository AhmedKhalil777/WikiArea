using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record UpdateWikiPageCommand : IRequest<WikiPageDto>
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? FolderId { get; init; }
    public bool IsPublic { get; init; } = true;
    public List<string> Tags { get; init; } = new();
    public List<string> AllowedRoles { get; init; } = new();
} 