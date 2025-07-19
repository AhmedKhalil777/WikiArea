using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WikiArea.Core.Interfaces;
using WikiArea.Infrastructure.Configuration;
using WikiArea.Infrastructure.Data;
using WikiArea.Infrastructure.Repositories;
using WikiArea.Infrastructure.Services;

namespace WikiArea.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);
        services.AddRepositories();
        services.AddAuthentication(configuration);
        services.AddCurrentUser();

        return services;
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.AddSingleton<WikiAreaMongoContext>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWikiPageRepository, WikiPageRepository>();
        services.AddScoped<IWikiFolderRepository, WikiFolderRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Primary JWT configuration for local authentication
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddOpenIdConnect(options =>
        {
            options.ClientId = configuration["Authentication:ClientId"];
            options.ClientSecret = configuration["Authentication:ClientSecret"];
            options.Authority = configuration["Authentication:Authority"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireReaderRole", policy => policy.RequireRole("Reader", "Writer", "Reviewer", "Administrator"));
            options.AddPolicy("RequireWriterRole", policy => policy.RequireRole("Writer", "Reviewer", "Administrator"));
            options.AddPolicy("RequireReviewerRole", policy => policy.RequireRole("Reviewer", "Administrator"));
            options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }

    private static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<DataSeedingService>();

        return services;
    }
} 