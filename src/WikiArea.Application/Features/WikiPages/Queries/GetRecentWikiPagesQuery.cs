using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Queries;

public record GetRecentWikiPagesQuery(int Count = 10) : IRequest<IEnumerable<WikiPageSummaryDto>>; 