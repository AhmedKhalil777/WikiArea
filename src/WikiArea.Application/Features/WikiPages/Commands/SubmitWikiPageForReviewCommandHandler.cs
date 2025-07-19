using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class SubmitWikiPageForReviewCommandHandler : IRequestHandler<SubmitWikiPageForReviewCommand, WikiPageDto>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public SubmitWikiPageForReviewCommandHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiPageDto> Handle(SubmitWikiPageForReviewCommand request, CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (wikiPage == null)
        {
            throw new NotFoundException($"Wiki page with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !wikiPage.CanEdit(currentUser))
        {
            throw new ForbiddenException("You do not have permission to submit this page for review.");
        }

        wikiPage.SubmitForReview(_currentUserService.UserId);
        await _wikiPageRepository.UpdateAsync(wikiPage, cancellationToken);

        return _mapper.Map<WikiPageDto>(wikiPage);
    }
} 