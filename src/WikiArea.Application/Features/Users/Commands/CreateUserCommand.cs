using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string DisplayName,
    string AdfsId,
    string Role,
    string Department
) : IRequest<UserDto>; 