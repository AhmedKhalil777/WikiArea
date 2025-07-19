using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiPages.Commands;

public class CreateWikiPageCommandHandler : IRequestHandler<CreateWikiPageCommand, WikiPageDto>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateWikiPageCommandHandler(
        IWikiPageRepository wikiPageRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiPageRepository = wikiPageRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiPageDto> Handle(CreateWikiPageCommand request, CancellationToken cancellationToken)
    {
        var contentType = ContentType.FromName(request.ContentType);
        
        var wikiPage = new WikiPage(
            request.Title,
            request.Content,
            contentType,
            request.FolderId,
            request.IsPublic)
        {
            CreatedBy = _currentUserService.UserId,
            UpdatedBy = _currentUserService.UserId
        };

        foreach (var tag in request.Tags)
        {
            wikiPage.AddTag(tag);
        }

        if (request.AllowedRoles.Any())
        {
            wikiPage.SetAllowedRoles(request.AllowedRoles);
        }

        await _wikiPageRepository.AddAsync(wikiPage, cancellationToken);
        
        return _mapper.Map<WikiPageDto>(wikiPage);
    }
} 