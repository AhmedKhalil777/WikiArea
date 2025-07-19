using Ardalis.SmartEnum;

namespace WikiArea.Core.Enums;

public sealed class PageStatus : SmartEnum<PageStatus>
{
    public static readonly PageStatus Draft = new(nameof(Draft), 1);
    public static readonly PageStatus UnderReview = new(nameof(UnderReview), 2);
    public static readonly PageStatus Published = new(nameof(Published), 3);
    public static readonly PageStatus Archived = new(nameof(Archived), 4);

    private PageStatus(string name, int value) : base(name, value) { }
} 