using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class UpdateWikiPageCommandHandler : IRequestHandler<UpdateWikiPageCommand, WikiPageDto>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateWikiPageCommandHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiPageDto> Handle(UpdateWikiPageCommand request, CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (wikiPage == null)
        {
            throw new NotFoundException($"Wiki page with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !wikiPage.CanEdit(currentUser))
        {
            throw new ForbiddenException("You do not have permission to edit this page.");
        }

        wikiPage.UpdateContent(request.Title, request.Content, _currentUserService.UserId);

        if (request.FolderId != wikiPage.FolderId)
        {
            wikiPage.Move(request.FolderId);
        }

        foreach (var tag in wikiPage.Tags.ToList())
        {
            if (!request.Tags.Contains(tag))
            {
                wikiPage.RemoveTag(tag);
            }
        }

        foreach (var tag in request.Tags)
        {
            wikiPage.AddTag(tag);
        }

        wikiPage.SetAllowedRoles(request.AllowedRoles);

        await _wikiPageRepository.UpdateAsync(wikiPage, cancellationToken);

        return _mapper.Map<WikiPageDto>(wikiPage);
    }
} 