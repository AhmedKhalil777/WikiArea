using Ardalis.GuardClauses;
using WikiArea.Core.ValueObjects;
using WikiArea.Core.Enums;

namespace WikiArea.Core.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public string AdfsId { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public string Department { get; private set; }
    public string AvatarUrl { get; private set; }
    public DateTime LastLoginAt { get; private set; }
    public List<string> Permissions { get; private set; } = new();
    
    // Password-based authentication fields
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public string AuthProvider { get; private set; } = "ADFS"; // ADFS or Local

    // ADFS authentication constructor
    public User(string username, string email, string displayName, string adfsId, UserRole role, string department)
    {
        Username = Guard.Against.NullOrWhiteSpace(username);
        Email = Guard.Against.NullOrWhiteSpace(email);
        DisplayName = Guard.Against.NullOrWhiteSpace(displayName);
        AdfsId = Guard.Against.NullOrWhiteSpace(adfsId);
        Role = role;
        Department = Guard.Against.NullOrWhiteSpace(department);
        Status = UserStatus.Active;
        AvatarUrl = string.Empty;
        LastLoginAt = DateTime.UtcNow;
        AuthProvider = "ADFS";
        SetDefaultPermissions();
    }
    
    // Local authentication constructor
    public User(string username, string email, string displayName, UserRole role, string department, string passwordHash, string passwordSalt)
    {
        Username = Guard.Against.NullOrWhiteSpace(username);
        Email = Guard.Against.NullOrWhiteSpace(email);
        DisplayName = Guard.Against.NullOrWhiteSpace(displayName);
        Role = role;
        Department = Guard.Against.NullOrWhiteSpace(department);
        PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash);
        PasswordSalt = Guard.Against.NullOrWhiteSpace(passwordSalt);
        Status = UserStatus.Active;
        AvatarUrl = string.Empty;
        AdfsId = string.Empty;
        LastLoginAt = DateTime.UtcNow;
        AuthProvider = "Local";
        SetDefaultPermissions();
    }

    private User() 
    {
        Username = string.Empty;
        Email = string.Empty;
        DisplayName = string.Empty;
        AdfsId = string.Empty;
        Role = UserRole.Reader; // Default role
        Status = UserStatus.Active;
        Department = string.Empty;
        AvatarUrl = string.Empty;
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
        AuthProvider = "ADFS";
        Permissions = new List<string>();
    }

    public void UpdateProfile(string displayName, string department, string avatarUrl)
    {
        DisplayName = Guard.Against.NullOrWhiteSpace(displayName);
        Department = Guard.Against.NullOrWhiteSpace(department);
        AvatarUrl = avatarUrl ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
        SetDefaultPermissions();
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLastLogin(DateTime lastLogin)
    {
        LastLoginAt = lastLogin;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasPermission(string permission) => Permissions.Contains(permission);
    
    public void UpdatePassword(string passwordHash, string passwordSalt)
    {
        PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash);
        PasswordSalt = Guard.Against.NullOrWhiteSpace(passwordSalt);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsLocalAuthUser() => AuthProvider == "Local";
    
    public bool IsActive => Status == UserStatus.Active;

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
        SetDefaultPermissions();
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDefaultPermissions()
    {
        Permissions.Clear();
        
        if (Role == UserRole.Reader)
        {
            Permissions.AddRange(new[] { "read:pages", "comment:pages" });
        }
        else if (Role == UserRole.Writer)
        {
            Permissions.AddRange(new[] { "read:pages", "write:pages", "comment:pages", "create:pages" });
        }
        else if (Role == UserRole.Reviewer)
        {
            Permissions.AddRange(new[] { "read:pages", "write:pages", "comment:pages", "create:pages", "review:pages", "approve:changes" });
        }
        else if (Role == UserRole.Administrator)
        {
            Permissions.AddRange(new[] { "*" });
        }
    }
} 