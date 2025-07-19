using MediatR;

namespace WikiArea.Core.Events;

public record WikiPageContentUpdatedEvent(string PageId, int Version, string UpdatedBy) : INotification;

public record WikiPagePublishedEvent(string PageId, string PublishedBy) : INotification;

public record WikiPageSubmittedForReviewEvent(string PageId, string SubmittedBy) : INotification;

public record WikiPageReviewApprovedEvent(string PageId, string ReviewerId, string Notes) : INotification;

public record WikiPageReviewRejectedEvent(string PageId, string ReviewerId, string Notes) : INotification;

public record WikiPageArchivedEvent(string PageId) : INotification;

public record WikiPageMovedEvent(string PageId, string? NewFolderId) : INotification; 