using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Comments.Commands;

public record ResolveCommentCommand(string Id) : IRequest<CommentDto>; 