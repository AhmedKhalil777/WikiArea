namespace WikiArea.Application.DTOs;

public class WikiFolderDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? ParentFolderId { get; set; }
    public int SortOrder { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public List<string> AllowedRoles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public WikiFolderDto? ParentFolder { get; set; }
    public List<WikiFolderDto> SubFolders { get; set; } = new();
    public List<WikiPageSummaryDto> Pages { get; set; } = new();
}

public class WikiFolderTreeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? ParentFolderId { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublic { get; set; }
    public List<WikiFolderTreeDto> Children { get; set; } = new();
    public int PageCount { get; set; }
} 