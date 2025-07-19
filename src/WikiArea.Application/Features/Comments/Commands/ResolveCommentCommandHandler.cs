using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Comments.Commands;

public class ResolveCommentCommandHandler : IRequestHandler<ResolveCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ResolveCommentCommandHandler(
        ICommentRepository commentRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<CommentDto> Handle(ResolveCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
        {
            throw new NotFoundException($"Comment with ID '{request.Id}' was not found.");
        }

        var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
        if (currentUser == null || !currentUser.HasPermission("resolve:comments"))
        {
            throw new ForbiddenException("You do not have permission to resolve comments.");
        }

        comment.Resolve(_currentUserService.UserId);
        await _commentRepository.UpdateAsync(comment, cancellationToken);

        return _mapper.Map<CommentDto>(comment);
    }
} 