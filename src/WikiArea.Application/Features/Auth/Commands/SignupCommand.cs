using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.Auth.Commands;

public class SignupCommand : IRequest<AuthResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
} 