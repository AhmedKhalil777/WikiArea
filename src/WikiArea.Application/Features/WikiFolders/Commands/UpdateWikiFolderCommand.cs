using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiFolders.Commands;

public record UpdateWikiFolderCommand(
    string Id,
    string Name,
    string Description,
    bool IsPublic
) : IRequest<WikiFolderDto>; 