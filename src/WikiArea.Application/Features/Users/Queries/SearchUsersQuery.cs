using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Users.Queries;

public record SearchUsersQuery(string SearchTerm) : IRequest<IEnumerable<UserSummaryDto>>; 