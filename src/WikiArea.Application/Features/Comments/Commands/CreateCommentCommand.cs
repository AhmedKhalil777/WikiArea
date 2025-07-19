using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Comments.Commands;

public record CreateCommentCommand(
    string WikiPageId,
    string Content,
    string? ParentCommentId = null
) : IRequest<CommentDto>; 