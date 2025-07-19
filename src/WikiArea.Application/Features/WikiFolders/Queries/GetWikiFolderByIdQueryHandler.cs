using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiFolders.Queries;

public class GetWikiFolderByIdQueryHandler : IRequestHandler<GetWikiFolderByIdQuery, WikiFolderDto?>
{
    private readonly IWikiFolderRepository _wikiFolderRepository;
    private readonly IMapper _mapper;

    public GetWikiFolderByIdQueryHandler(
        IWikiFolderRepository wikiFolderRepository,
        IMapper mapper)
    {
        _wikiFolderRepository = wikiFolderRepository;
        _mapper = mapper;
    }

    public async Task<WikiFolderDto?> Handle(GetWikiFolderByIdQuery request, CancellationToken cancellationToken)
    {
        var wikiFolder = await _wikiFolderRepository.GetByIdAsync(request.Id, cancellationToken);
        return wikiFolder != null ? _mapper.Map<WikiFolderDto>(wikiFolder) : null;
    }
} 