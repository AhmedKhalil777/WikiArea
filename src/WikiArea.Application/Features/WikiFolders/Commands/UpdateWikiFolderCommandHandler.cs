using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiFolders.Commands;

public class UpdateWikiFolderCommandHandler : IRequestHandler<UpdateWikiFolderCommand, WikiFolderDto>
{
    private readonly IWikiFolderRepository _wikiFolderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateWikiFolderCommandHandler(
        IWikiFolderRepository wikiFolderRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _wikiFolderRepository = wikiFolderRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<WikiFolderDto> Handle(UpdateWikiFolderCommand request, CancellationToken cancellationToken)
    {
        var wikiFolder = await _wikiFolderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (wikiFolder == null)
        {
            throw new NotFoundException($"Wiki folder with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !currentUser.HasPermission("write:folders"))
        {
            throw new ForbiddenException("You do not have permission to update folders.");
        }

        wikiFolder.UpdateDetails(request.Name, request.Description, request.IsPublic);
        wikiFolder.UpdatedBy = _currentUserService.UserId;
        
        await _wikiFolderRepository.UpdateAsync(wikiFolder, cancellationToken);

        return _mapper.Map<WikiFolderDto>(wikiFolder);
    }
} 