using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Entities;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiFolders.Commands;

public class CreateWikiFolderCommandHandler : IRequestHandler<CreateWikiFolderCommand, WikiFolderDto>
{
    private readonly IWikiFolderRepository _wikiFolderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateWikiFolderCommandHandler(
        IWikiFolderRepository wikiFolderRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiFolderRepository = wikiFolderRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiFolderDto> Handle(CreateWikiFolderCommand request, CancellationToken cancellationToken)
    {
        var wikiFolder = new WikiFolder(
            request.Name,
            request.Description,
            request.Path,
            request.ParentFolderId,
            request.IsPublic)
        {
            CreatedBy = _currentUserService.UserId,
            UpdatedBy = _currentUserService.UserId
        };

        await _wikiFolderRepository.AddAsync(wikiFolder, cancellationToken);
        
        return _mapper.Map<WikiFolderDto>(wikiFolder);
    }
} 