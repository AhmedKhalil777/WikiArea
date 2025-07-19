using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Queries;

public class GetWikiPageBySlugQueryHandler : IRequestHandler<GetWikiPageBySlugQuery, WikiPageDto?>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetWikiPageBySlugQueryHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiPageDto?> Handle(GetWikiPageBySlugQuery request, CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetBySlugAsync(request.Slug, cancellationToken);
        if (wikiPage == null)
        {
            return null;
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !wikiPage.HasAccess(currentUser))
        {
            throw new ForbiddenException("You do not have permission to access this page.");
        }

        wikiPage.IncrementViewCount();
        await _wikiPageRepository.UpdateAsync(wikiPage, cancellationToken);

        return _mapper.Map<WikiPageDto>(wikiPage);
    }
} 