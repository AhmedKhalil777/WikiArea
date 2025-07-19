using MediatR;

namespace WikiArea.Application.Features.WikiPages.Commands;

public record DeleteWikiPageCommand(string Id) : IRequest; 