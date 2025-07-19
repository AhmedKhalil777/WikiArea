using MongoDB.Driver;
using WikiArea.Core.Entities;
using WikiArea.Core.Interfaces;
using WikiArea.Infrastructure.Data;

namespace WikiArea.Infrastructure.Repositories;

public class WikiFolderRepository : MongoRepository<WikiFolder>, IWikiFolderRepository
{
    public WikiFolderRepository(WikiAreaMongoContext context) : base(context.WikiFolders)
    {
    }

    public async Task<WikiFolder?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Path == path && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiFolder>> GetByParentIdAsync(string? parentId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.ParentFolderId == parentId && !x.IsDeleted)
            .SortBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiFolder>> GetRootFoldersAsync(CancellationToken cancellationToken = default)
    {
        return await GetByParentIdAsync(null, cancellationToken);
    }

    public async Task<IEnumerable<WikiFolder>> GetDescendantsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        var descendants = new List<WikiFolder>();
        var children = await GetByParentIdAsync(folderId, cancellationToken);
        
        foreach (var child in children)
        {
            descendants.Add(child);
            var grandChildren = await GetDescendantsAsync(child.Id, cancellationToken);
            descendants.AddRange(grandChildren);
        }
        
        return descendants;
    }

    public async Task<IEnumerable<WikiFolder>> GetAncestorsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        var ancestors = new List<WikiFolder>();
        var folder = await GetByIdAsync(folderId, cancellationToken);
        
        while (folder?.ParentFolderId != null)
        {
            var parent = await GetByIdAsync(folder.ParentFolderId, cancellationToken);
            if (parent != null)
            {
                ancestors.Insert(0, parent);
                folder = parent;
            }
            else
            {
                break;
            }
        }
        
        return ancestors;
    }

    public async Task<bool> IsPathUniqueAsync(string path, string? excludeFolderId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WikiFolder>.Filter.And(
            Builders<WikiFolder>.Filter.Eq(x => x.Path, path),
            Builders<WikiFolder>.Filter.Eq(x => x.IsDeleted, false)
        );

        if (!string.IsNullOrEmpty(excludeFolderId))
        {
            filter = Builders<WikiFolder>.Filter.And(filter, 
                Builders<WikiFolder>.Filter.Ne(x => x.Id, excludeFolderId));
        }

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count == 0;
    }

    public async Task<bool> HasChildrenAsync(string folderId, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(
            x => x.ParentFolderId == folderId && !x.IsDeleted, 
            cancellationToken: cancellationToken);
        return count > 0;
    }
} 