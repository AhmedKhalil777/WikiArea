using MediatR;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class DeleteWikiPageCommandHandler : IRequestHandler<DeleteWikiPageCommand>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteWikiPageCommandHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteWikiPageCommand request, CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (wikiPage == null)
        {
            throw new NotFoundException($"Wiki page with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !wikiPage.CanEdit(currentUser))
        {
            throw new ForbiddenException("You do not have permission to delete this page.");
        }

        await _wikiPageRepository.DeleteAsync(request.Id, cancellationToken);
    }
} 