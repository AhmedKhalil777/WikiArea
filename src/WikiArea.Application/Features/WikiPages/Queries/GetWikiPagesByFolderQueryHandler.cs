using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Queries;

public class GetWikiPagesByFolderQueryHandler : IRequestHandler<GetWikiPagesByFolderQuery, IEnumerable<WikiPageSummaryDto>>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetWikiPagesByFolderQueryHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WikiPageSummaryDto>> Handle(GetWikiPagesByFolderQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        var wikiPages = await _wikiPageRepository.GetByFolderIdAsync(request.FolderId, cancellationToken);

        var accessiblePages = wikiPages.Where(page => currentUser != null && page.HasAccess(currentUser));
        return _mapper.Map<IEnumerable<WikiPageSummaryDto>>(accessiblePages);
    }
} 