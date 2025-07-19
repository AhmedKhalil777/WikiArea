using MediatR;

namespace WikiArea.Core.Events;

public record CommentCreatedEvent(string CommentId, string WikiPageId, string AuthorId, List<string> Mentions) : INotification;

public record CommentUpdatedEvent(string CommentId, string WikiPageId, string UpdatedBy) : INotification;

public record CommentResolvedEvent(string CommentId, string WikiPageId, string ResolvedBy) : INotification; 