using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Queries;

public record GetWikiPagesByFolderQuery(string? FolderId) : IRequest<IEnumerable<WikiPageSummaryDto>>; 