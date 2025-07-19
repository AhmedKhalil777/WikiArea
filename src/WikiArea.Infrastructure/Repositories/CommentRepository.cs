using MongoDB.Driver;
using WikiArea.Core.Entities;
using WikiArea.Core.Interfaces;
using WikiArea.Infrastructure.Data;

namespace WikiArea.Infrastructure.Repositories;

public class CommentRepository : MongoRepository<Comment>, ICommentRepository
{
    public CommentRepository(WikiAreaMongoContext context) : base(context.Comments)
    {
    }

    public async Task<IEnumerable<Comment>> GetByWikiPageIdAsync(string wikiPageId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.WikiPageId == wikiPageId && !x.IsDeleted)
            .SortBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetByAuthorIdAsync(string authorId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.AuthorId == authorId && !x.IsDeleted)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetRepliesAsync(string parentCommentId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.ParentCommentId == parentCommentId && !x.IsDeleted)
            .SortBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetUnresolvedAsync(string wikiPageId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.WikiPageId == wikiPageId && !x.IsResolved && !x.IsDeleted)
            .SortBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetByMentionAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Mentions.Contains(username) && !x.IsDeleted)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCommentCountAsync(string wikiPageId, CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(
            x => x.WikiPageId == wikiPageId && !x.IsDeleted, 
            cancellationToken: cancellationToken);
    }
} 