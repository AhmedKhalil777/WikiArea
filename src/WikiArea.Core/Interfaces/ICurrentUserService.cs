using WikiArea.Core.Entities;

namespace WikiArea.Core.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string Username { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    bool HasPermission(string permission);
    bool IsInRole(string role);
} 