using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;

namespace WikiArea.Infrastructure.Services;

public class DataSeedingService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(IUserRepository userRepository, ILogger<DataSeedingService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task SeedDefaultDataAsync()
    {
        await SeedDefaultAdminUserAsync();
    }

    private async Task SeedDefaultAdminUserAsync()
    {
        // Check if admin user already exists by email
        var existingAdmin = await _userRepository.GetByEmailAsync("progeng_ahmed_khalil@outlook.com");
        if (existingAdmin != null)
        {
            _logger.LogInformation("Admin user already exists, skipping seeding");
            return; // Admin already exists
        }

        // Also check if any admin users exist at all
        var existingUsers = await _userRepository.GetAllAsync();
        var adminExists = existingUsers.Any(u => u.Role.Name == "Administrator");
        if (adminExists)
        {
            _logger.LogInformation("Administrator user already exists, skipping default admin seeding");
            return;
        }

        // Create default admin user
        var (passwordHash, salt) = HashPassword("ahmed777khalil");
        
        var adminUser = new User(
            username: "admin",
            email: "progeng_ahmed_khalil@outlook.com",
            displayName: "Ahmed Khalil (Admin)",
            role: UserRole.Administrator,
            department: "IT Administration",
            passwordHash: passwordHash,
            passwordSalt: salt
        );

        // The constructor already sets Status to Active for local auth users
        // But let's explicitly ensure it's active
        adminUser.Activate();
        
        // Debug logging
        _logger.LogInformation("Created admin user - Email: {Email}, Status: {Status}, IsActive: {IsActive}", 
            adminUser.Email, adminUser.Status.Name, adminUser.IsActive);

        // Set custom permissions for admin
        adminUser.Permissions.Clear();
        adminUser.Permissions.AddRange(new[]
        {
            "*", // All permissions
            "admin:manage_users",
            "admin:manage_permissions", 
            "admin:manage_system",
            "admin:view_admin_panel"
        });

        await _userRepository.AddAsync(adminUser);
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        // Generate a random salt
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        var salt = Convert.ToBase64String(saltBytes);

        // Hash the password using PBKDF2 with 10,000 iterations
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        var hash = Convert.ToBase64String(hashBytes);

        return (hash, salt);
    }
} 