using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Queries;

public record SearchWikiPagesQuery(string SearchTerm) : IRequest<IEnumerable<WikiPageSummaryDto>>; 