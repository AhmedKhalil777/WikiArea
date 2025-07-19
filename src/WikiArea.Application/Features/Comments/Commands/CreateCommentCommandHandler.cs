using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Entities;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Comments.Commands;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment(
            request.WikiPageId,
            _currentUserService.UserId,
            request.Content,
            request.ParentCommentId)
        {
            CreatedBy = _currentUserService.UserId,
            UpdatedBy = _currentUserService.UserId
        };

        await _commentRepository.AddAsync(comment, cancellationToken);
        
        return _mapper.Map<CommentDto>(comment);
    }
} 