using WikiArea.Core.Entities;

namespace WikiArea.Core.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByWikiPageIdAsync(string wikiPageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetByAuthorIdAsync(string authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetRepliesAsync(string parentCommentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetUnresolvedAsync(string wikiPageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetByMentionAsync(string username, CancellationToken cancellationToken = default);
    Task<int> GetCommentCountAsync(string wikiPageId, CancellationToken cancellationToken = default);
} 