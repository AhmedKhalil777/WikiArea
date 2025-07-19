using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Users.Queries;

public record GetUserByIdQuery(string Id) : IRequest<UserDto?>; 