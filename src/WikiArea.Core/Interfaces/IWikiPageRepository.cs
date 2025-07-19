using WikiArea.Core.Entities;
using WikiArea.Core.Enums;

namespace WikiArea.Core.Interfaces;

public interface IWikiPageRepository : IRepository<WikiPage>
{
    Task<WikiPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetByFolderIdAsync(string? folderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetByStatusAsync(PageStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetMostViewedAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiPage>> GetForReviewAsync(CancellationToken cancellationToken = default);
    Task<bool> IsSlugUniqueAsync(string slug, string? excludePageId = null, CancellationToken cancellationToken = default);
} 