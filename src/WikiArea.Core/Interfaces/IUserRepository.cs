using WikiArea.Core.Entities;
using WikiArea.Core.Enums;

namespace WikiArea.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default);
    Task<User?> GetByAdfsIdAsync(string adfsId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, string? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default);
} 