using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WikiArea.Core.Entities;
using WikiArea.Core.Interfaces;

namespace WikiArea.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public string Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public async Task<User?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
        {
            return null;
        }

        return await _userRepository.GetByIdAsync(UserId, cancellationToken);
    }

    public bool HasPermission(string permission)
    {
        if (!IsAuthenticated)
        {
            return false;
        }

        return _httpContextAccessor.HttpContext?.User?.FindFirstValue("permission")?.Contains(permission) ?? false;
    }

    public bool IsInRole(string role)
    {
        if (!IsAuthenticated)
        {
            return false;
        }

        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
} 