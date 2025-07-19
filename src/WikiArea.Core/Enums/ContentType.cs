using Ardalis.SmartEnum;

namespace WikiArea.Core.Enums;

public sealed class ContentType : SmartEnum<ContentType>
{
    public static readonly ContentType Markdown = new(nameof(Markdown), 1, "text/markdown", ".md");
    public static readonly ContentType Html = new(nameof(Html), 2, "text/html", ".html");
    public static readonly ContentType PlainText = new(nameof(PlainText), 3, "text/plain", ".txt");
    public static readonly ContentType Video = new(nameof(Video), 4, "video/*", ".mp4");
    public static readonly ContentType Image = new(nameof(Image), 5, "image/*", ".jpg");
    public static readonly ContentType Document = new(nameof(Document), 6, "application/*", ".pdf");

    public string MimeType { get; }
    public string DefaultExtension { get; }

    private ContentType(string name, int value, string mimeType, string defaultExtension) : base(name, value)
    {
        MimeType = mimeType;
        DefaultExtension = defaultExtension;
    }
} 