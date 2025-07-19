using Ardalis.GuardClauses;
using WikiArea.Core.Events;

namespace WikiArea.Core.Entities;

public class WikiFolder : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Path { get; private set; }
    public string? ParentFolderId { get; private set; }
    public int SortOrder { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public bool IsPublic { get; private set; }
    public List<string> AllowedRoles { get; private set; } = new();

    public WikiFolder(string name, string description, string path, string? parentFolderId = null, bool isPublic = true)
    {
        Name = Guard.Against.NullOrWhiteSpace(name);
        Description = description ?? string.Empty;
        Path = Guard.Against.NullOrWhiteSpace(path);
        ParentFolderId = parentFolderId;
        IsPublic = isPublic;
        SortOrder = 0;
        
        RegisterDomainEvent(new WikiFolderCreatedEvent(Id, Name, Path));
    }

    private WikiFolder() 
    {
        Name = null!;
        Description = null!;
        Path = null!;
    }

    public void UpdateDetails(string name, string description, bool isPublic)
    {
        Name = Guard.Against.NullOrWhiteSpace(name);
        Description = description ?? string.Empty;
        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePath(string newPath)
    {
        var oldPath = Path;
        Path = Guard.Against.NullOrWhiteSpace(newPath);
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiFolderPathChangedEvent(Id, oldPath, newPath));
    }

    public void Move(string? newParentFolderId)
    {
        ParentFolderId = newParentFolderId;
        UpdatedAt = DateTime.UtcNow;
        
        RegisterDomainEvent(new WikiFolderMovedEvent(Id, newParentFolderId));
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
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
} 