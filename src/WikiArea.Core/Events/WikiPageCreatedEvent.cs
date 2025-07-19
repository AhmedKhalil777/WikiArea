using MediatR;

namespace WikiArea.Core.Events;

public record WikiPageCreatedEvent(string PageId, string Title, string Slug) : INotification; 