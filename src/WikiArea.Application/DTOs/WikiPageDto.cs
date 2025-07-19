using WikiArea.Core.ValueObjects;

namespace WikiArea.Application.DTOs;

public class WikiPageDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? FolderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Version { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Attachments { get; set; } = new();
    public bool IsPublic { get; set; }
    public List<string> AllowedRoles { get; set; } = new();
    public string? ReviewerId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string ReviewNotes { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public PageMetadata Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public UserDto? Author { get; set; }
    public UserDto? Reviewer { get; set; }
    public WikiFolderDto? Folder { get; set; }
}

public class WikiPageSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Version { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public string? FolderId { get; set; }
    public List<string> Tags { get; set; } = new();
} 