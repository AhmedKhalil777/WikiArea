using MediatR;

namespace WikiArea.Core.Events;

public record WikiFolderCreatedEvent(string FolderId, string Name, string Path) : INotification;

public record WikiFolderPathChangedEvent(string FolderId, string OldPath, string NewPath) : INotification;

public record WikiFolderMovedEvent(string FolderId, string? NewParentFolderId) : INotification; 