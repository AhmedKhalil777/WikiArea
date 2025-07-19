using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record PublishWikiPageCommand(string Id) : IRequest<WikiPageDto>; 