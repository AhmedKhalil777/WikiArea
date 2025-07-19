using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Auth.Commands;

public class SigninCommand : IRequest<AuthResponse>
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
} 