using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.WikiFolders.Queries;

public class GetRootFoldersQueryHandler : IRequestHandler<GetRootFoldersQuery, IEnumerable<WikiFolderDto>>
{
    private readonly IWikiFolderRepository _wikiFolderRepository;
    private readonly IMapper _mapper;

    public GetRootFoldersQueryHandler(
        IWikiFolderRepository wikiFolderRepository,
        IMapper mapper)
    {
        _wikiFolderRepository = wikiFolderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WikiFolderDto>> Handle(GetRootFoldersQuery request, CancellationToken cancellationToken)
    {
        var folders = await _wikiFolderRepository.GetRootFoldersAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WikiFolderDto>>(folders);
    }
} 