using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Comments.Queries;

public class GetCommentsByPageQueryHandler : IRequestHandler<GetCommentsByPageQuery, IEnumerable<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public GetCommentsByPageQueryHandler(
        ICommentRepository commentRepository,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByPageQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.GetByWikiPageIdAsync(request.WikiPageId, cancellationToken);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
} 