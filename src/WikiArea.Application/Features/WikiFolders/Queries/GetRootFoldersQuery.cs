using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiFolders.Queries;

public record GetRootFoldersQuery() : IRequest<IEnumerable<WikiFolderDto>>; 