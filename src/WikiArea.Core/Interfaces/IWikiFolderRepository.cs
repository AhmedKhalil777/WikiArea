using WikiArea.Core.Entities;

namespace WikiArea.Core.Interfaces;

public interface IWikiFolderRepository : IRepository<WikiFolder>
{
    Task<WikiFolder?> GetByPathAsync(string path, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiFolder>> GetByParentIdAsync(string? parentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiFolder>> GetRootFoldersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiFolder>> GetDescendantsAsync(string folderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WikiFolder>> GetAncestorsAsync(string folderId, CancellationToken cancellationToken = default);
    Task<bool> IsPathUniqueAsync(string path, string? excludeFolderId = null, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(string folderId, CancellationToken cancellationToken = default);
} 