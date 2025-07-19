using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record ApproveWikiPageReviewCommand(string Id, string Notes) : IRequest<WikiPageDto>; 