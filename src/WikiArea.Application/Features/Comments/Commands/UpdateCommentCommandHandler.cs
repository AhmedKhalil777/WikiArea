using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Comments.Commands;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
        {
            throw new NotFoundException($"Comment with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || comment.AuthorId != _currentUserService.UserId)
        {
            throw new ForbiddenException("You can only edit your own comments.");
        }

        comment.UpdateContent(request.Content, _currentUserService.UserId);
        await _commentRepository.UpdateAsync(comment, cancellationToken);

        return _mapper.Map<CommentDto>(comment);
    }
} 