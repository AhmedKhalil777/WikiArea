using MongoDB.Driver;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;
using WikiArea.Infrastructure.Data;

namespace WikiArea.Infrastructure.Repositories;

public class WikiPageRepository : MongoRepository<WikiPage>, IWikiPageRepository
{
    public WikiPageRepository(WikiAreaMongoContext context) : base(context.WikiPages)
    {
    }

    public async Task<WikiPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Slug == slug && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetByFolderIdAsync(string? folderId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.FolderId == folderId && !x.IsDeleted)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Tags.Contains(tag.ToLowerInvariant()) && !x.IsDeleted)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetByStatusAsync(PageStatus status, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Status == status && !x.IsDeleted)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.CreatedBy == authorId && !x.IsDeleted)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WikiPage>.Filter.And(
            Builders<WikiPage>.Filter.Text(searchTerm),
            Builders<WikiPage>.Filter.Eq(x => x.IsDeleted, false)
        );

        return await _collection.Find(filter)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => !x.IsDeleted)
            .SortByDescending(x => x.UpdatedAt)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetMostViewedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => !x.IsDeleted)
            .SortByDescending(x => x.ViewCount)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WikiPage>> GetForReviewAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Status == PageStatus.UnderReview && !x.IsDeleted)
            .SortBy(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, string? excludePageId = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WikiPage>.Filter.And(
            Builders<WikiPage>.Filter.Eq(x => x.Slug, slug),
            Builders<WikiPage>.Filter.Eq(x => x.IsDeleted, false)
        );

        if (!string.IsNullOrEmpty(excludePageId))
        {
            filter = Builders<WikiPage>.Filter.And(filter, 
                Builders<WikiPage>.Filter.Ne(x => x.Id, excludePageId));
        }

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count == 0;
    }
} 