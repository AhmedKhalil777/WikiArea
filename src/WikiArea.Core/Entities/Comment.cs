using Ardalis.GuardClauses;
using WikiArea.Core.Events;

namespace WikiArea.Core.Entities;

public class Comment : BaseEntity
{
    public string WikiPageId { get; private set; }
    public string AuthorId { get; private set; }
    public string Content { get; private set; }
    public string? ParentCommentId { get; private set; }
    public bool IsResolved { get; private set; }
    public string? ResolvedBy { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public List<string> Mentions { get; private set; } = new();
    public int LikeCount { get; private set; }

    public Comment(string wikiPageId, string authorId, string content, string? parentCommentId = null)
    {
        WikiPageId = Guard.Against.NullOrWhiteSpace(wikiPageId);
        AuthorId = Guard.Against.NullOrWhiteSpace(authorId);
        Content = Guard.Against.NullOrWhiteSpace(content);
        ParentCommentId = parentCommentId;
        IsResolved = false;
        LikeCount = 0;
        
        ExtractMentions();
        RegisterDomainEvent(new CommentCreatedEvent(Id, WikiPageId, AuthorId, Mentions));
    }

    private Comment() 
    {
        WikiPageId = null!;
        AuthorId = null!;
        Content = null!;
    }

    public void UpdateContent(string newContent, string updatedBy)
    {
        Content = Guard.Against.NullOrWhiteSpace(newContent);
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
        
        ExtractMentions();
        RegisterDomainEvent(new CommentUpdatedEvent(Id, WikiPageId, updatedBy));
    }

    public void Resolve(string resolvedBy)
    {
        IsResolved = true;
        ResolvedBy = resolvedBy;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new CommentResolvedEvent(Id, WikiPageId, resolvedBy));
    }

    public void Unresolve()
    {
        IsResolved = false;
        ResolvedBy = null;
        ResolvedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementLikeCount()
    {
        LikeCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementLikeCount()
    {
        if (LikeCount > 0)
        {
            LikeCount--;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    private void ExtractMentions()
    {
        Mentions.Clear();
        
        var words = Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var mentions = words
            .Where(word => word.StartsWith('@') && word.Length > 1)
            .Select(word => word[1..].Trim(',', '.', '!', '?', ';', ':'))
            .Where(mention => !string.IsNullOrWhiteSpace(mention))
            .Distinct()
            .ToList();
            
        Mentions.AddRange(mentions);
    }
} 