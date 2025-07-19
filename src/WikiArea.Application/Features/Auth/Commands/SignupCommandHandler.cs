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

public class SignupCommandHandler : IRequestHandler<SignupCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public SignupCommandHandler(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        // Check if username or email already exists
        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(request.Username, request.Email);
        if (existingUser != null)
        {
            throw new BusinessValidationException("Username or email already exists");
        }

        // Hash password
        var (hashedPassword, salt) = HashPassword(request.Password);

        // Create user
        var user = new User(
            request.Username.ToLower(),
            request.Email.ToLower(), 
            request.DisplayName,
            UserRole.Writer,
            request.Department,
            hashedPassword,
            salt
        );

        await _userRepository.AddAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    private static (string hashedPassword, string salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashedPassword = Convert.ToBase64String(pbkdf2.GetBytes(32));

        return (hashedPassword, salt);
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