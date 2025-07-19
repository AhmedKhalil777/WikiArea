using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiFolders.Commands;

public record CreateWikiFolderCommand(
    string Name,
    string Description,
    string Path,
    string? ParentFolderId = null,
    bool IsPublic = true
) : IRequest<WikiFolderDto>; 