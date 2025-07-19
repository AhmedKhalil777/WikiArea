using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class ApproveWikiPageReviewCommandHandler : IRequestHandler<ApproveWikiPageReviewCommand, WikiPageDto>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ApproveWikiPageReviewCommandHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiPageDto> Handle(ApproveWikiPageReviewCommand request, CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (wikiPage == null)
        {
            throw new NotFoundException($"Wiki page with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !wikiPage.CanReview(currentUser))
        {
            throw new ForbiddenException("You do not have permission to review this page.");
        }

        wikiPage.ApproveReview(_currentUserService.UserId, request.Notes);
        await _wikiPageRepository.UpdateAsync(wikiPage, cancellationToken);

        return _mapper.Map<WikiPageDto>(wikiPage);
    }
} 