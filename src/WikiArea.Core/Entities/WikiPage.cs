using Ardalis.GuardClauses;
using WikiArea.Core.Enums;
using WikiArea.Core.Events;
using WikiArea.Core.ValueObjects;

namespace WikiArea.Core.Entities;

public class WikiPage : BaseEntity
{
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Content { get; private set; }
    public ContentType ContentType { get; private set; }
    public string? FolderId { get; private set; }
    public PageStatus Status { get; private set; }
    public int Version { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public List<string> Attachments { get; private set; } = new();
    public bool IsPublic { get; private set; }
    public List<string> AllowedRoles { get; private set; } = new();
    public string? ReviewerId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string ReviewNotes { get; private set; } = string.Empty;
    public int ViewCount { get; private set; }
    public int LikeCount { get; private set; }
    public PageMetadata Metadata { get; private set; }

    public WikiPage(string title, string content, ContentType contentType, string? folderId = null, bool isPublic = true)
    {
        Title = Guard.Against.NullOrWhiteSpace(title);
        Content = Guard.Against.NullOrWhiteSpace(content);
        ContentType = contentType;
        FolderId = folderId;
        IsPublic = isPublic;
        Status = PageStatus.Draft;
        Version = 1;
        ViewCount = 0;
        LikeCount = 0;
        Slug = GenerateSlug(title);
        Metadata = new PageMetadata();
        
        RegisterDomainEvent(new WikiPageCreatedEvent(Id, Title, Slug));
    }

    private WikiPage() 
    {
        Title = null!;
        Slug = null!;
        Content = null!;
        ContentType = null!;
        Status = PageStatus.Draft;
        Metadata = null!;
    }

    public void UpdateContent(string title, string content, string updatedBy)
    {
        Title = Guard.Against.NullOrWhiteSpace(title);
        Content = Guard.Against.NullOrWhiteSpace(content);
        Slug = GenerateSlug(title);
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
        Version++;
        
        if (Status == PageStatus.Published)
        {
            Status = PageStatus.Draft;
        }
        
        RegisterDomainEvent(new WikiPageContentUpdatedEvent(Id, Version, updatedBy));
    }

    public void Publish(string publishedBy)
    {
        Status = PageStatus.Published;
        UpdatedBy = publishedBy;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPagePublishedEvent(Id, publishedBy));
    }

    public void SubmitForReview(string submittedBy)
    {
        Status = PageStatus.UnderReview;
        UpdatedBy = submittedBy;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPageSubmittedForReviewEvent(Id, submittedBy));
    }

    public void ApproveReview(string reviewerId, string notes = "")
    {
        ReviewerId = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        Status = PageStatus.Published;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPageReviewApprovedEvent(Id, reviewerId, notes));
    }

    public void RejectReview(string reviewerId, string notes)
    {
        ReviewerId = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = Guard.Against.NullOrWhiteSpace(notes);
        Status = PageStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPageReviewRejectedEvent(Id, reviewerId, notes));
    }

    public void Archive()
    {
        Status = PageStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPageArchivedEvent(Id));
    }

    public void Move(string? newFolderId)
    {
        FolderId = newFolderId;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiPageMovedEvent(Id, newFolderId));
    }

    public void AddTag(string tag)
    {
        var normalizedTag = tag.Trim().ToLowerInvariant();
        if (!Tags.Contains(normalizedTag))
        {
            Tags.Add(normalizedTag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTag(string tag)
    {
        var normalizedTag = tag.Trim().ToLowerInvariant();
        if (Tags.Remove(normalizedTag))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void AddAttachment(string attachmentId)
    {
        if (!Attachments.Contains(attachmentId))
        {
            Attachments.Add(attachmentId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveAttachment(string attachmentId)
    {
        if (Attachments.Remove(attachmentId))
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void IncrementViewCount()
    {
        ViewCount++;
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

    public void SetAllowedRoles(IEnumerable<string> roles)
    {
        AllowedRoles.Clear();
        AllowedRoles.AddRange(roles);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAccess(User user)
    {
        if (IsPublic) return true;
        if (user.Role.Name == "Administrator") return true;
        return AllowedRoles.Contains(user.Role.Name);
    }

    public bool CanEdit(User user)
    {
        if (!HasAccess(user)) return false;
        return user.HasPermission("write:pages") || user.HasPermission("*");
    }

    public bool CanReview(User user)
    {
        return user.HasPermission("review:pages") || user.HasPermission("*");
    }

    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Guid.NewGuid().ToString("N")[..8]; // Fallback to random string
        }

        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("?", "")
            .Replace("&", "and")
            .Replace("@", "at")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("!", "")
            .Replace("#", "")
            .Replace("$", "")
            .Replace("%", "")
            .Replace("^", "")
            .Replace("*", "")
            .Replace("+", "")
            .Replace("=", "")
            .Replace("|", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("~", "")
            .Replace("`", "");

        // Remove multiple consecutive dashes
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        // Remove leading and trailing dashes
        slug = slug.Trim('-');

        // If slug becomes empty after cleanup, use a fallback
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = Guid.NewGuid().ToString("N")[..8];
        }

        // Ensure it's not too long
        if (slug.Length > 100)
        {
            slug = slug[..100].TrimEnd('-');
        }

        return slug;
    }
} 