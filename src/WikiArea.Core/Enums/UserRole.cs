using Ardalis.SmartEnum;

namespace WikiArea.Core.Enums;

public sealed class UserRole : SmartEnum<UserRole>
{
    public static readonly UserRole Reader = new(nameof(Reader), 1);
    public static readonly UserRole Writer = new(nameof(Writer), 2);
    public static readonly UserRole Reviewer = new(nameof(Reviewer), 3);
    public static readonly UserRole Administrator = new(nameof(Administrator), 4);

    private UserRole(string name, int value) : base(name, value) { }
} 