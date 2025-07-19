using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Comments.Commands;

public record UpdateCommentCommand(
    string Id,
    string Content
) : IRequest<CommentDto>; 