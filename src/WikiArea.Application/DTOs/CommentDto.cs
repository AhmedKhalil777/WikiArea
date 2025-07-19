namespace WikiArea.Application.DTOs;

public class CommentDto
{
    public string Id { get; set; } = string.Empty;
    public string WikiPageId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<string> Mentions { get; set; } = new();
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public UserSummaryDto? Author { get; set; }
    public UserSummaryDto? ResolvedByUser { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
} 