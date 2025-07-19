using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WikiArea.Application.DTOs;
using WikiArea.Core.Interfaces;
using WikiArea.Core.Enums;
using WikiArea.Core.Entities;
using System.Security.Cryptography;

namespace WikiArea.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAdministratorRole")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IUserRepository userRepository, ILogger<AdminController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role.Name,
                Department = user.Department,
                Status = user.Status.Name,
                AuthProvider = user.AuthProvider,
                Permissions = user.Permissions,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            });

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update user role and permissions
    /// </summary>
    [HttpPut("users/{userId}/role")]
    public async Task<ActionResult> UpdateUserRole(string userId, [FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Parse role
            var role = UserRole.FromName(request.Role);
            if (role == null)
            {
                return BadRequest("Invalid role specified");
            }

            // Update user role
            user.UpdateRole(role);

            // Update custom permissions if provided
            if (request.CustomPermissions != null && request.CustomPermissions.Any())
            {
                user.Permissions.Clear();
                user.Permissions.AddRange(request.CustomPermissions);
            }

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} role updated to {Role} by admin", userId, request.Role);
            return Ok(new { message = "User role updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Activate or deactivate user
    /// </summary>
    [HttpPut("users/{userId}/status")]
    public async Task<ActionResult> UpdateUserStatus(string userId, [FromBody] UpdateUserStatusRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (request.IsActive)
            {
                user.Activate();
            }
            else
            {
                user.Deactivate();
            }

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} status updated to {Status} by admin", userId, request.IsActive ? "Active" : "Inactive");
            return Ok(new { message = "User status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost("users")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            // Parse role
            var role = UserRole.FromName(request.Role);
            if (role == null)
            {
                return BadRequest("Invalid role specified");
            }

            // Hash password if provided
            string? passwordHash = null;
            string? salt = null;
            if (!string.IsNullOrEmpty(request.Password))
            {
                (passwordHash, salt) = HashPassword(request.Password);
            }

            // Create user
            var user = new User(
                username: request.Username,
                email: request.Email,
                displayName: request.DisplayName,
                role: role,
                department: request.Department ?? "Unknown",
                passwordHash: passwordHash ?? string.Empty,
                passwordSalt: salt ?? string.Empty
            );

            user.Activate();

            await _userRepository.AddAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role.Name,
                Department = user.Department,
                Status = user.Status.Name,
                AuthProvider = user.AuthProvider,
                Permissions = user.Permissions,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };

            _logger.LogInformation("New user {Username} created by admin", request.Username);
            return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Username}", request.Username);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("users/{userId}")]
    public async Task<ActionResult> DeleteUser(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Prevent deletion of the default admin
            if (user.Email == "progeng_ahmed_khalil@outlook.com")
            {
                return BadRequest("Cannot delete the default administrator account");
            }

            await _userRepository.DeleteAsync(userId);

            _logger.LogInformation("User {UserId} deleted by admin", userId);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get available roles and permissions
    /// </summary>
    [HttpGet("roles-permissions")]
    public ActionResult<object> GetRolesAndPermissions()
    {
        var roles = new[]
        {
            new { Name = "Reader", Value = UserRole.Reader.Name },
            new { Name = "Writer", Value = UserRole.Writer.Name },
            new { Name = "Reviewer", Value = UserRole.Reviewer.Name },
            new { Name = "Administrator", Value = UserRole.Administrator.Name }
        };

        var permissions = new[]
        {
            "read:pages",
            "write:pages", 
            "comment:pages",
            "create:pages",
            "review:pages",
            "approve:changes",
            "admin:manage_users",
            "admin:manage_permissions",
            "admin:manage_system",
            "admin:view_admin_panel",
            "*" // All permissions
        };

        return Ok(new { roles, permissions });
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

// Request DTOs
public class UpdateUserRoleRequest
{
    public string Role { get; set; } = string.Empty;
    public List<string>? CustomPermissions { get; set; }
}

public class UpdateUserStatusRequest
{
    public bool IsActive { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Password { get; set; }
    public string? AuthProvider { get; set; }
} 