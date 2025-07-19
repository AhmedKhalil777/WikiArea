using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record SubmitWikiPageForReviewCommand(string Id) : IRequest<WikiPageDto>; 