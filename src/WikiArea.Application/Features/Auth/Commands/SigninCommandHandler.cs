using MediatR;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WikiArea.Application.DTOs;
using WikiArea.Application.Exceptions;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Auth.Commands;

public class SigninCommandHandler : IRequestHandler<SigninCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public SigninCommandHandler(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Handle(SigninCommand request, CancellationToken cancellationToken)
    {
        // Find user by username or email
        var user = await _userRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail, request.UsernameOrEmail);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        // Check if user is active
        if (user.Status != UserStatus.Active)
        {
            throw new UnauthorizedException("Account is not active");
        }

        // Update last login
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    private static bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            var computedHash = Convert.ToBase64String(pbkdf2.GetBytes(32));
            return computedHash == hashedPassword;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("displayName", user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("department", user.Department),
            new Claim("authProvider", user.AuthProvider)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 