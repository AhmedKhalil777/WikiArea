using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record RejectWikiPageReviewCommand(string Id, string Notes) : IRequest<WikiPageDto>; 