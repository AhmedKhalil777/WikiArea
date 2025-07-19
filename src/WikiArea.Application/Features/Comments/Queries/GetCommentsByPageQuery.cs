using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Comments.Queries;

public record GetCommentsByPageQuery(string WikiPageId) : IRequest<IEnumerable<CommentDto>>; 