using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Comments.Queries;

public record GetCommentByIdQuery(string Id) : IRequest<CommentDto?>; 